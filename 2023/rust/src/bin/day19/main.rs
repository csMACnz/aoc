use std::{collections::HashMap, fmt::Error, fs, str::FromStr};

#[derive(Debug, PartialEq, Eq, Hash)]
struct WorkflowName(String);

#[derive(Debug)]
struct Cogs {
    rules: HashMap<WorkflowName, (Vec<Rule>, Action)>,
    parts: Vec<Part>,
}

#[derive(Debug)]
enum Category {
    X,
    M,
    A,
    S,
}

#[derive(Debug)]
enum Comparison {
    LessThan,
    GreaterThan,
}

#[derive(Debug)]
struct Rule {
    category: Category,
    comparison: Comparison,
    value: u64,
    action: Action,
}

#[derive(Debug, PartialEq, Eq)]
struct ParseError(&'static str);

impl FromStr for Rule {
    type Err = ParseError;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let (rule, action) = s.split_once(":").ok_or(ParseError("Missing :"))?;
        let category = match rule.chars().nth(0) {
            Some('x') => Category::X,
            Some('m') => Category::M,
            Some('a') => Category::A,
            Some('s') => Category::S,
            _ => return Err(ParseError("Bad Category")),
        };

        let comparison = match rule.chars().nth(1) {
            Some('<') => Comparison::LessThan,
            Some('>') => Comparison::GreaterThan,
            _ => return Err(ParseError("Bad Comparison")),
        };
        Ok(Rule {
            action: action
                .parse::<Action>()
                .map_err(|_| ParseError("Bad action"))?,
            category,
            comparison,
            value: rule[2..]
                .parse::<u64>()
                .map_err(|_| ParseError("Bad value"))?,
        })
    }
}

#[derive(Debug)]
enum Action {
    Workflow(WorkflowName),
    Outcome(Outcome),
}

impl FromStr for Action {
    type Err = Error;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        match s {
            "A" => Ok(Action::Outcome(Outcome::Accepted)),
            "R" => Ok(Action::Outcome(Outcome::Rejected)),
            s => Ok(Action::Workflow(WorkflowName(s.into()))),
        }
    }
}

#[derive(Debug, PartialEq, Eq, Clone, Copy)]
enum Outcome {
    Rejected,
    Accepted,
}

#[derive(Debug)]
struct Part {
    x: u64,
    m: u64,
    a: u64,
    s: u64,
}

impl Part {
    fn get(&self, category: &Category) -> u64 {
        match category {
            Category::X => self.x,
            Category::M => self.m,
            Category::A => self.a,
            Category::S => self.s,
        }
    }

    fn sum(&self) -> u64 {
        self.x + self.m + self.a + self.s
    }
}

#[derive(Debug, Clone)]
struct PartRanges {
    x: (u64, u64),
    m: (u64, u64),
    a: (u64, u64),
    s: (u64, u64),
}

fn split(
    min_max: (u64, u64),
    comparison: &Comparison,
    value: u64,
) -> (Option<(u64, u64)>, Option<(u64, u64)>) {
    assert!(min_max.1 >= min_max.0);
    match comparison {
        Comparison::LessThan => {
            if min_max.1 < value {
                (Some(min_max), None)
            } else if min_max.0 >= value {
                (None, Some(min_max))
            } else {
                (Some((min_max.0, value - 1)), Some((value, min_max.1)))
            }
        }
        Comparison::GreaterThan => {
            if min_max.1 <= value {
                (Some(min_max), None)
            } else if min_max.0 > value {
                (None, Some(min_max))
            } else {
                (Some((min_max.0, value)), Some((value + 1, min_max.1)))
            }
        }
    }
}

impl PartRanges {
    fn init() -> PartRanges {
        PartRanges {
            x: (1, 4000),
            m: (1, 4000),
            a: (1, 4000),
            s: (1, 4000),
        }
    }

    fn get(&self, category: &Category) -> (u64, u64) {
        match category {
            Category::X => self.x,
            Category::M => self.m,
            Category::A => self.a,
            Category::S => self.s,
        }
    }

    fn with(&self, category: &Category, range: (u64, u64)) -> PartRanges {
        let mut result: PartRanges = self.clone();
        match category {
            Category::X => result.x = range,
            Category::M => result.m = range,
            Category::A => result.a = range,
            Category::S => result.s = range,
        }
        result
    }

    fn split(
        &self,
        category: &Category,
        comparison: &Comparison,
        value: u64,
    ) -> (Option<Self>, Option<Self>) {
        let (left, right) = split(self.get(&category), comparison, value);

        (
            left.and_then(|l| Some(self.with(&category, l))),
            right.and_then(|r| Some(self.with(&category, r))),
        )
    }

    fn score(&self) -> u64 {
        let result = (self.x.1 - self.x.0 + 1)
            * (self.m.1 - self.m.0 + 1)
            * (self.a.1 - self.a.0 + 1)
            * (self.s.1 - self.s.0 + 1);
        result
    }
}

impl FromStr for Part {
    type Err = Error;

    fn from_str(input: &str) -> Result<Self, Self::Err> {
        let mut iter = input.chars();
        assert!(iter.next().is_some_and(|c| c == '{'));

        let mut x = 0;
        assert!(iter.next().is_some_and(|c| c == 'x'));
        assert!(iter.next().is_some_and(|c| c == '='));
        let mut num = 0_u64;
        loop {
            let c = iter.next().unwrap();
            if c == ',' {
                x = num;
                break;
            } else {
                num = (num * 10) + c.to_digit(10).unwrap() as u64;
            }
        }
        let mut m = 0;
        assert!(iter.next().is_some_and(|c| c == 'm'));
        assert!(iter.next().is_some_and(|c| c == '='));
        let mut num = 0_u64;
        loop {
            let c = iter.next().unwrap();
            if c == ',' {
                m = num;
                break;
            } else {
                num = (num * 10) + c.to_digit(10).unwrap() as u64;
            }
        }
        assert!(iter.next().is_some_and(|c| c == 'a'));
        let mut a = 0;
        assert!(iter.next().is_some_and(|c| c == '='));
        let mut num = 0_u64;
        loop {
            let c = iter.next().unwrap();
            if c == ',' {
                a = num;
                break;
            } else {
                num = (num * 10) + c.to_digit(10).unwrap() as u64;
            }
        }
        let mut s = 0;
        assert!(iter.next().is_some_and(|c| c == 's'));
        assert!(iter.next().is_some_and(|c| c == '='));
        let mut num = 0_u64;
        loop {
            let c = iter.next().unwrap();
            if c == '}' {
                s = num;
                break;
            } else {
                num = (num * 10) + c.to_digit(10).unwrap() as u64;
            }
        }

        Ok(Part { x, m, a, s })
    }
}

fn parse_file(path: &str) -> Cogs {
    let lines = fs::read_to_string(path).expect("Should have been able to read the file");
    let mut lines_iter = lines.lines();

    let mut rules = HashMap::new();
    while let Some(line) = lines_iter.next() {
        if line.is_empty() {
            break;
        }
        let (key, line) = line.split_once("{").unwrap();
        let x = line.trim_end_matches("}").split(",").collect::<Vec<_>>();
        let (&fallback, parts) = x.split_last().unwrap();
        let rule_parts = parts
            .iter()
            .map(|&a| a.parse::<Rule>().unwrap())
            .collect::<Vec<Rule>>();
        rules.insert(
            WorkflowName(key.into()),
            (rule_parts, fallback.parse::<Action>().unwrap()),
        );
    }

    let parts = lines_iter.map(|l| l.parse::<Part>().unwrap()).collect();

    Cogs { rules, parts }
}

fn process(cogs: &Cogs, part: &Part, workflow_name: &WorkflowName) -> Outcome {
    let (rules, fallback) = cogs.rules.get(workflow_name).unwrap();
    let mut result: &Action = fallback;
    for rule in rules {
        let matches = match rule.comparison {
            Comparison::LessThan => part.get(&rule.category) < rule.value,
            Comparison::GreaterThan => part.get(&rule.category) > rule.value,
        };

        if matches {
            result = &rule.action;
            break;
        }
    }
    match result {
        Action::Workflow(name) => process(cogs, part, name),
        Action::Outcome(outcome) => *outcome,
    }
}

fn part_1(path: &str) -> u64 {
    let cogs = parse_file(path);
    let initial_workflow = WorkflowName("in".into());
    let mut result = 0;
    for part in &cogs.parts {
        let outcome = process(&cogs, &part, &initial_workflow);
        if outcome == Outcome::Accepted {
            result += part.sum();
        }
    }
    result
}

fn score(cogs: &Cogs, ranges: PartRanges, action: &Action) -> u64 {
    match action {
        Action::Workflow(workflow_name) => count_accepted_outcomes(&cogs, ranges, &workflow_name),
        Action::Outcome(Outcome::Accepted) => ranges.score(),
        Action::Outcome(Outcome::Rejected) => 0,
    }
}

fn count_accepted_outcomes(cogs: &Cogs, ranges: PartRanges, workflow_name: &WorkflowName) -> u64 {
    let (rules, fallback) = cogs.rules.get(workflow_name).unwrap();
    let mut accepted_outcomes = 0;
    let mut next = ranges;
    for rule in rules {
        let (left_range, right_range) = next.split(&rule.category, &rule.comparison, rule.value);
        // recurse one side, and iterate the other
        let (action_side, continue_side) = match rule.comparison {
            Comparison::LessThan => (left_range, right_range),
            Comparison::GreaterThan => (right_range, left_range),
        };
        if let Some(action_side) = action_side {
            accepted_outcomes += score(cogs, action_side, &rule.action);
        };
        if let Some(continue_side) = continue_side {
            next = continue_side;
            continue;
        } else {
            return accepted_outcomes;
        }
    }
    accepted_outcomes += score(cogs, next, &fallback);
    accepted_outcomes
}

fn part_2(path: &str) -> u64 {
    let cogs = parse_file(path);
    let initial_workflow = WorkflowName("in".into());
    count_accepted_outcomes(&cogs, PartRanges::init(), &initial_workflow)
}

fn main() {
    let answer1 = part_1("./src/bin/day19/puzzle.txt");
    let answer2 = part_2("./src/bin/day19/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample() {
    let answer = part_1("./src/bin/day19/sample.txt");

    assert_eq!(answer, 19114);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day19/puzzle.txt");

    assert_eq!(answer, 432434);
}

#[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day19/sample.txt");
    assert_eq!(answer, 167409079868000);
}

#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day19/puzzle.txt");

    assert_eq!(answer, 132557544578569);
}
