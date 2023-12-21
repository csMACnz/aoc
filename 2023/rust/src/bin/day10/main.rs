use std::{collections::VecDeque, fs};

struct Node {
    symbol: char,
    visited: bool,
    neighbours: Vec<usize>,
}

struct Graph {
    start: usize,
    x_dim: usize,
    y_dim: usize,
    nodes: Vec<Node>,
}

impl Graph {
    pub fn at_index(self: &Self, y: usize, x: usize) -> &Node {
        &self.nodes[self.index(y, x)]
    }
    pub fn at_index_mut(self: &mut Self, y: usize, x: usize) -> &mut Node {
        let idx = self.index(y, x);
        &mut self.nodes[idx]
    }
    fn index(self: &Self, y: usize, x: usize) -> usize {
        assert!(y < self.y_dim);
        assert!(x < self.x_dim);
        y * self.x_dim + x
    }
    fn char_at(self: &Self, y: usize, x: usize) -> char {
        self.at_index(y, x).symbol
    }
    fn mark(self: &mut Self, idx: usize) {
        self.nodes[idx].visited = true;
    }

    fn neighbours<'a>(self: &'a Self, idx: usize) -> std::slice::Iter<'a, usize> {
        self.nodes[idx].neighbours.iter()
    }
}

fn gen_start_neighbours(graph: &Graph, start_y: usize, start_x: usize) -> Vec<usize> {
    let mut result = Vec::new();
    if start_x > 0
        && ['-', 'L', 'F']
            .iter()
            .any(|p| *p == graph.char_at(start_y, start_x - 1))
    {
        result.push(graph.index(start_y, start_x - 1));
    }
    if start_x < graph.x_dim - 1
        && ['-', 'J', '7']
            .iter()
            .any(|p| *p == graph.char_at(start_y, start_x + 1))
    {
        result.push(graph.index(start_y, start_x + 1));
    }
    if start_y > 0
        && ['|', '7', 'F']
            .iter()
            .any(|p| *p == graph.char_at(start_y - 1, start_x))
    {
        result.push(graph.index(start_y - 1, start_x));
    }
    if start_y < graph.y_dim - 1
        && ['|', 'L', 'J']
            .iter()
            .any(|p| *p == graph.char_at(start_y + 1, start_x))
    {
        result.push(graph.index(start_y + 1, start_x));
    }
    // println!("S: {result:?}");
    result
}

fn gen_neighbours(graph: &Graph, y: usize, x: usize) -> Vec<usize> {
    let mut result = Vec::new();
    let c = graph.char_at(y, x);
    if x > 0
        && ['-', 'J', '7'].iter().any(|p| *p == c)
        && ['-', 'L', 'F', 'S']
            .iter()
            .any(|p| *p == graph.char_at(y, x - 1))
    {
        result.push(graph.index(y, x - 1));
    }
    if x < graph.x_dim - 1
        && ['-', 'F', 'L'].iter().any(|p| *p == c)
        && ['-', 'J', '7', 'S']
            .iter()
            .any(|p| *p == graph.char_at(y, x + 1))
    {
        result.push(graph.index(y, x + 1));
    }
    if y > 0
        && ['|', 'L', 'J'].iter().any(|p| *p == c)
        && ['|', '7', 'F', 'S']
            .iter()
            .any(|p| *p == graph.char_at(y - 1, x))
    {
        result.push(graph.index(y - 1, x));
    }
    if y < graph.y_dim - 1
        && ['|', '7', 'F'].iter().any(|p| *p == c)
        && ['|', 'L', 'J', 'S']
            .iter()
            .any(|p| *p == graph.char_at(y + 1, x))
    {
        result.push(graph.index(y + 1, x));
    }
    result
}

fn parse_file(path: &str) -> Graph {
    let file_contents = fs::read_to_string(path).expect("Should have been able to read the file");

    let grid: Vec<Node> = file_contents
        .lines()
        .flat_map(|l| {
            l.chars()
                .map(|c| Node {
                    symbol: c,
                    visited: false,
                    neighbours: Vec::new(),
                })
                .collect::<Vec<Node>>()
        })
        .collect();

    let mut graph = Graph {
        start: 0,
        nodes: grid,
        y_dim: file_contents.lines().count(),
        x_dim: file_contents.lines().next().unwrap().len(),
    };

    assert!(graph.nodes.len() == (graph.y_dim * graph.x_dim));

    let y_dim = graph.y_dim;
    let x_dim = graph.x_dim;
    for y in 0..y_dim {
        for x in 0..x_dim {
            let mut neighbours = if graph.char_at(y, x) == 'S' {
                graph.start = graph.index(y, x);
                gen_start_neighbours(&graph, y, x)
            } else {
                gen_neighbours(&graph, y, x)
            };
            graph.at_index_mut(y, x).neighbours.append(&mut neighbours);
        }
    }

    for y in 0..graph.y_dim {
        for x in 0..graph.x_dim {
            let at_index = graph.at_index(y, x);
            for neighbour in at_index.neighbours.clone() {
                // println!("({y},{x}) {:?}", neighbour);
                assert!(graph.nodes[neighbour]
                    .neighbours
                    .contains(&graph.index(y, x)))
            }
        }
    }

    graph
}

fn part_1(path: &str) -> i64 {
    let mut graph = parse_file(path);

    let mut queue = VecDeque::new();

    for n in graph.neighbours(graph.start) {
        queue.push_back((graph.start, *n, 1));
    }
    graph.mark(graph.start);
    while queue.len() > 0 {
        let (prev, curr, depth) = queue.pop_front().unwrap();
        graph.mark(curr);
        for n in graph.neighbours(curr) {
            if *n == prev {
                continue;
            }

            if graph.nodes[*n].visited {
                return depth;
            }

            queue.push_back((curr, *n, depth + 1));
        }
    }
    0
}

fn main() {
    let answer = part_1("./src/bin/day10/puzzle.txt");

    println!("Part1: {}", answer);

    println!("Part2: {}", "<Unknown>");
}

#[test]
fn can_parse_part1_sample1() {
    let answer = part_1("./src/bin/day10/part1_sample1.txt");

    assert_eq!(answer, 4);
}
#[test]
fn can_parse_part1_sample2() {
    let answer = part_1("./src/bin/day10/part1_sample2.txt");

    assert_eq!(answer, 8);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day10/puzzle.txt");

    assert_eq!(answer, 6907);
}
