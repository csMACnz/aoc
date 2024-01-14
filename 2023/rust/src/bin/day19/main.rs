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

fn part_2(path: &str) -> u64 {
    let cogs = parse_file(path);
    0
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

// #[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day19/sample.txt");

    assert_eq!(answer, 0);
}

// #[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day19/puzzle.txt");

    assert_eq!(answer, 0);
}
