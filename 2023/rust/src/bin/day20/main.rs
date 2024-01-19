use std::{
    collections::{HashMap, VecDeque},
    fs,
};

#[derive(Debug)]
struct Graph {
    data: HashMap<&'static str, Node>,
}

impl Graph {
    fn push_button(&mut self) -> HashMap<&'static str, (u64, u64)> {
        let mut results = HashMap::new();
        let mut queue = VecDeque::new();
        queue.push_back(("button", "broadcaster", Pulse::Low));
        while let Some((source, dest, pulse)) = queue.pop_front() {
            let source_pulse_count = results.entry(source).or_insert((0, 0));
            match pulse {
                Pulse::High => source_pulse_count.0 += 1,
                Pulse::Low => source_pulse_count.1 += 1,
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
                        let next_pulse = if inputs.iter().all(|(_, &p)| p == Pulse::High) {
                            Pulse::Low
                        } else {
                            Pulse::High
                        };
                        Some(next_pulse)
                    }
                };
                if let Some(pulse) = signal {
                    node.destinations
                        .iter()
                        .for_each(|d| queue.push_back((node.name, d, pulse)));
                }
            }
        }
        results
    }

    fn get_source_nodes(&self, name: &str) -> Vec<&Node> {
        self.data
            .iter()
            .filter(|(_, n)| n.destinations.contains(&name))
            .map(|(_, n)| n)
            .collect()
    }
}

#[derive(Debug)]
struct Node {
    name: &'static str,
    node_type: NodeType,
    destinations: Vec<&'static str>,
}

impl From<&'static str> for Node {
    fn from(s: &'static str) -> Self {
        //broadcaster -> a, b, c
        let (node_type_name, destinations) = s.split_once(" -> ").expect("No -> found");
        let (node_type, name) = match node_type_name.chars().next().expect("No first char found") {
            'b' => (NodeType::Broadcaster, node_type_name),
            '%' => (NodeType::FlipFlop(false), &node_type_name[1..]),
            '&' => (NodeType::Conjunction(HashMap::new()), &node_type_name[1..]),
            _ => unreachable!(),
        };
        let destinations = destinations
            .to_string()
            .leak::<'static>()
            .split(", ")
            .collect();

        Self {
            name: name,
            node_type,
            destinations,
        }
    }
}

#[derive(Debug, PartialEq, Eq)]
enum NodeType {
    Broadcaster,
    FlipFlop(bool),                            // on|off
    Conjunction(HashMap<&'static str, Pulse>), // last_pulse
}

#[derive(Debug, Copy, Clone, PartialEq, Eq)]
enum Pulse {
    High,
    Low,
}

fn parse_file(path: &str) -> Graph {
    let read_to_string: &str = fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .leak();

    let mut data: HashMap<&str, Node> = read_to_string
        .lines()
        .map(|l| {
            let n: Node = l.into();
            (n.name, n)
        })
        .into_iter()
        .collect();

    let mut conjunctions: HashMap<&str, HashMap<&str, Pulse>> = HashMap::new();
    for (&node_key, node) in data.iter() {
        for &node_destination in node.destinations.iter() {
            if let Some(dest_node) = data.get(node_destination) {
                if let NodeType::Conjunction(_) = dest_node.node_type {
                    if let Some(map) = conjunctions.get_mut(dest_node.name) {
                        map.insert(node_key, Pulse::Low);
                    } else {
                        let mut map = HashMap::new();
                        map.insert(node_key, Pulse::Low);
                        conjunctions.insert(dest_node.name, map);
                    }
                }
            }
        }
    }
    for (conj_name, map) in conjunctions {
        let conj_node = data.get_mut(conj_name).unwrap();
        if let NodeType::Conjunction(_) = &conj_node.node_type {
            conj_node.node_type = NodeType::Conjunction(map);
        } else {
            unreachable!();
        }
    }
    Graph { data }
}

fn lcm(n1: u64, n2: u64) -> u64 {
    let mut x;
    let mut y;
    let mut rem;

    if n1 > n2 {
        x = n1;
        y = n2;
    } else {
        x = n2;
        y = n1;
    }

    rem = x % y;

    while rem != 0 {
        x = y;
        y = rem;
        rem = x % y;
    }
    let lcm = n1 * n2 / y;
    lcm
}
fn part_1(path: &str) -> u64 {
    let mut graph = parse_file(path);
    let mut low_pulse_count = 0;
    let mut high_pulse_count = 0;
    for _ in 0..1000 {
        let (l, h) = graph
            .push_button()
            .values()
            .fold((0, 0), |(al, ah), (el, eh)| (al + el, ah + eh));
        low_pulse_count += l;
        high_pulse_count += h;
    }
    low_pulse_count * high_pulse_count
}

fn part_2(path: &str, out_signal: &str) -> u64 {
    let mut graph = parse_file(path);
    let last_node = graph.get_source_nodes(out_signal);
    assert!(last_node.len() == 1);
    let last_node_name = last_node[0].name;
    assert!(matches!(last_node[0].node_type, NodeType::Conjunction(_)));
    let parents = graph.get_source_nodes(last_node_name);
    let all_parents_are_conjunctions = parents
        .iter()
        .all(|x| matches!(x.node_type, NodeType::Conjunction(_)));

    if all_parents_are_conjunctions {
        let mut iterations: HashMap<&str, u64> = HashMap::new();
        let mut count = 0;
        for parent in parents {
            iterations.insert(parent.name, 0);
        }
        loop {
            let results = graph.push_button();
            count += 1;

            let parents = graph.get_source_nodes(last_node_name);
            for parent in parents {
                if *iterations.get(parent.name).unwrap() == 0 {
                    if let Some((l, _)) = results.get(parent.name) {
                        if *l > 0 {
                            iterations.insert(parent.name, count);
                        }
                    }
                }
            }
            if iterations.iter().all(|(_, v)| *v != 0) {
                break iterations
                    .values()
                    .copied()
                    .reduce(|acc, c| lcm(acc, c))
                    .unwrap();
            }
        }
    } else {
        let mut count = 0;
        loop {
            let results = graph.push_button();
            count += 1;
            if let Some((l, _)) = results.get(last_node_name) {
                if *l > 0 {
                    break;
                }
            }
        }
        count
    }
    //use the type to work out the iteration counts for Lowest Common, recurse
    // calc_required_iterations_to_fire_low(&graph, last_node[0]).0
}


fn main() {
    let answer1 = part_1("./src/bin/day20/puzzle.txt");
    let answer2 = part_2("./src/bin/day20/puzzle.txt", "rx");

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

    assert_eq!(answer, 929810733);
}

#[test]
fn can_parse_part2_sample_2() {
    let answer = part_2("./src/bin/day20/sample_2.txt", "output");
    assert_eq!(answer, 1);
}

#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day20/puzzle.txt", "rx");

    assert_eq!(answer, 231657829136023);
}
