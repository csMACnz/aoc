use std::{
    cmp::Reverse,
    collections::{HashMap, HashSet, VecDeque},
    fs,
};

struct Graph {
    nodes: HashMap<String, Vec<String>>,
}
impl Graph {
    fn remove_edge(&mut self, a: &String, b: &String) {
        self.nodes.get_mut(a).unwrap().retain(|x| x != b);
        self.nodes.get_mut(b).unwrap().retain(|x| x != a);
    }

    fn get_neighbours(&self, key: &String) -> &Vec<String> {
        self.nodes.get(key).unwrap()
    }

    fn calculate_group_size(&self, start: &String) -> u64 {
        let mut seen = HashSet::new();
        let mut queue = VecDeque::new();
        queue.push_back(start);
        while let Some(next) = queue.pop_front() {
            if seen.insert(next) {
                for dest in self.nodes.get(next).unwrap() {
                    queue.push_back(dest);
                }
            }
        }
        seen.len() as u64
    }
}

fn parse_file(path: &str) -> Graph {
    let mut nodes = HashMap::new();

    // read in nodes from file (bidirectional tree, with gaps.)
    for line in fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .lines()
    {
        let (src, dests) = line.split_once(": ").unwrap();
        let dests: Vec<String> = dests.split(" ").map(str::to_string).collect();
        nodes.insert(src.to_string(), dests);
    }
    //ensure graph is two ways
    let mut to_insert = Vec::new();
    for k in nodes.keys() {
        let dests = nodes.get(k).unwrap();
        for d in dests {
            match nodes.get(d) {
                Some(vec) => {
                    if !vec.contains(k) {
                        to_insert.push((d.to_owned(), k.to_owned()));
                    }
                }
                None => to_insert.push((d.to_owned(), k.to_owned())),
            }
        }
    }
    // insert missing bi-directionals
    for (k, v) in to_insert {
        match nodes.get_mut(&k) {
            Some(vec) => {
                vec.push(v);
            }
            None => {
                nodes.insert(k, vec![v]);
            }
        }
    }

    // validate is a valid set of nodes
    for k in nodes.keys() {
        let dests = nodes.get(k).unwrap();
        for d in dests {
            assert!(nodes.get(d).unwrap().contains(k));
        }
    }
    Graph { nodes }
}

fn part_1(path: &str) -> u64 {
    let mut graph = parse_file(path);
    let mut trace_count = HashMap::new();
    for key in graph.nodes.keys() {
        let mut seen = HashSet::new();
        let mut queue = VecDeque::new();
        queue.push_back(vec![key]);
        while let Some(path) = queue.pop_front() {
            let next = path[0];
            if seen.insert(next) {
                //process path up to next;
                for window in path.windows(2) {
                    let key = (window[0].min(window[1]), window[0].max(window[1]));
                    let count = trace_count.entry(key).or_insert(0);
                    *count += 1;
                }
                // push next's nexts
                for dest in graph.get_neighbours(next) {
                    let mut to_push = path.clone();
                    to_push.insert(0, dest);
                    queue.push_back(to_push);
                }
            }
        }
    }
    let mut v: Vec<(Reverse<u64>, (String, String))> = trace_count
        .iter()
        .map(|(k, &v)| (Reverse(v), (k.0.clone(), k.1.clone())))
        .collect();
    v.sort();
    println!("{:?} {:?} {:?}", v[0], v[1], v[2]);
    for (a, b) in [&v[0].1, &v[1].1, &v[2].1] {
        graph.remove_edge(a, b);
    }
    let group_a_count = graph.calculate_group_size(&v[0].1 .0);
    let group_b_count = graph.calculate_group_size(&v[0].1 .1);
    group_a_count * group_b_count
}

fn main() {
    let answer1 = part_1("./src/bin/day25/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("No Part 2 for day 25!");
}

#[test]
fn can_parse_part1_sample() {
    let answer = part_1("./src/bin/day25/sample.txt");

    assert_eq!(answer, 54);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day25/puzzle.txt");

    assert_eq!(answer, 601344);
}
