use std::{
    collections::{HashMap, VecDeque},
    fs,
    str::FromStr,
};

#[derive(Debug)]
struct Graph {
    data: HashMap<String, Node>,
    low_pulses: u64,
    high_pulses: u64,
}

impl Graph {
    fn score(&self) -> u64 {
        self.low_pulses * self.high_pulses
    }

    fn push_button(&mut self) {
        let mut queue = VecDeque::new();
        queue.push_back(("button".to_string(), "broadcaster".to_string(), Pulse::Low));
        while let Some((source, dest, pulse)) = queue.pop_front() {
            match pulse {
                Pulse::High => self.high_pulses += 1,
                Pulse::Low => self.low_pulses += 1,
            }
            if let Some(node) = self.data.get_mut(&dest) {
                let signal: Option<Pulse> = match node.node_type {
                    NodeType::Broadcaster => Some(pulse),
                    NodeType::FlipFlop(state) => match (pulse, state) {
                        (Pulse::High, _) => None,
                        (Pulse::Low, true) => {
                            node.node_type = NodeType::FlipFlop(false);
                            Some(Pulse::Low)
                        }
                        (Pulse::Low, false) => {
                            node.node_type = NodeType::FlipFlop(true);
                            Some(Pulse::High)
                        }
                    },
                    NodeType::Conjunction(ref mut inputs) => {
                        inputs.insert(source, pulse);
                        let next_pulse =
                            if inputs.iter().filter(|(_, &p)| p == Pulse::Low).count() == 0 {
                                Pulse::Low
                            } else {
                                Pulse::High
                            };
                        Some(next_pulse)
                    }
                };
                if let Some(pulse) = signal {
                    node.destinations.iter().for_each(|d| {
                        queue.push_back((node.name.to_string(), d.to_string(), pulse))
                    });
                }
            }
        }
    }
}

#[derive(Debug)]
struct Node {
    name: String,
    node_type: NodeType,
    destinations: Vec<String>,
}

impl FromStr for Node {
    type Err = String;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        //broadcaster -> a, b, c
        let (node_type_name, destinations) = s.split_once(" -> ").ok_or("No -> found")?;
        let (node_type, name) = match node_type_name.chars().next().ok_or("No first char found")? {
            'b' => (NodeType::Broadcaster, node_type_name),
            '%' => (NodeType::FlipFlop(false), &node_type_name[1..]),
            '&' => (NodeType::Conjunction(HashMap::new()), &node_type_name[1..]),
            _ => unreachable!(),
        };
        let destinations = destinations.split(", ").map(str::to_string).collect();

        Ok(Self {
            name: name.to_string(),
            node_type,
            destinations,
        })
    }
}

#[derive(Debug)]
enum NodeType {
    Broadcaster,
    FlipFlop(bool),                      // on|off
    Conjunction(HashMap<String, Pulse>), // last_pulse
}

#[derive(Debug, Copy, Clone, PartialEq, Eq)]
enum Pulse {
    High,
    Low,
}

fn parse_file(path: &str) -> Graph {
    let mut data: HashMap<String, Node> = fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .lines()
        .map(|l| {
            let n = l.parse::<Node>().unwrap();
            (n.name.to_owned(), n)
        })
        .into_iter()
        .collect();

    let mut conjunctions: HashMap<String, HashMap<String, Pulse>> = HashMap::new();
    for (node_key, node) in &data {
        for node_destination in &node.destinations {
            if let Some(dest_node) = data.get(node_destination) {
                if let NodeType::Conjunction(_) = dest_node.node_type {
                    if let Some(map) = conjunctions.get_mut(&dest_node.name) {
                        map.insert(node_key.to_string(), Pulse::Low);
                    } else {
                        let mut map = HashMap::new();
                        map.insert(node_key.to_string(), Pulse::Low);
                        conjunctions.insert(dest_node.name.to_string(), map);
                    }
                }
            }
        }
    }
    for (conj_name, map) in conjunctions {
        let conj_node = data.get_mut(&conj_name).unwrap();
        if let NodeType::Conjunction(_) = &conj_node.node_type {
            conj_node.node_type = NodeType::Conjunction(map);
        } else {
            unreachable!();
        }
    }
    Graph {
        data,
        low_pulses: 0,
        high_pulses: 0,
    }
}

fn part_1(path: &str) -> u64 {
    let mut graph = parse_file(path);
    for _ in 0..1000 {
        graph.push_button();
    }
    graph.score()
}

fn part_2(path: &str) -> u64 {
    let graph = parse_file(path);
    0
}

fn main() {
    let answer1 = part_1("./src/bin/day20/puzzle.txt");
    let answer2 = part_2("./src/bin/day20/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample_1() {
    let answer = part_1("./src/bin/day20/sample_1.txt");

    assert_eq!(answer, 32000000);
}
#[test]
fn can_parse_part1_sample_2() {
    let answer = part_1("./src/bin/day20/sample_2.txt");

    assert_eq!(answer, 11687500);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day20/puzzle.txt");

    assert_eq!(answer, 0);
}

// #[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day20/sample.txt");
    assert_eq!(answer, 0);
}

// #[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day20/puzzle.txt");

    assert_eq!(answer, 0);
}
