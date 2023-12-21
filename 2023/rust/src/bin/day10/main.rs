use std::{collections::VecDeque, fs};

#[derive(Clone, Debug)]
struct Node {
    x: usize,
    y: usize,
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

#[derive(Clone, Copy, PartialEq, Debug)]
enum FillState {
    Out,
    TopIn,
    BottomIn,
    In,
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

    fn solve_start_symbol(self: &mut Self, before: usize, after: usize) {
        let start_node = &self.nodes[self.start];
        let before_node = &self.nodes[before];
        let after_node = &self.nodes[after];
        let symbol = match (
            before_node.y as i64 - start_node.y as i64,
            before_node.x as i64 - start_node.x as i64,
            after_node.y as i64 - start_node.y as i64,
            after_node.x as i64 - start_node.x as i64,
        ) {
            (0, -1, 0, 1) | (0, 1, 0, -1) => '-',
            (0, 1, 1, 0) | (1, 0, 0, 1) => 'F',
            (-1, 0, 0, 1) | (0, 1, -1, 0) => 'L',
            (0, -1, 1, 0) | (1, 0, 0, -1) => '7',
            (-1, 0, 0, -1) | (0, -1, -1, 0) => 'J',
            (-1, 0, 1, 0) | (1, 0, -1, 0) => '|',
            _ => panic!("Dev Error"),
        };
        let start_node = &mut self.nodes[self.start];
        start_node.symbol = symbol;
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
                    x: 0,
                    y: 0,
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
            let node = graph.at_index_mut(y, x);
            node.neighbours.append(&mut neighbours);
            node.x = x;
            node.y = y;
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

fn part_2(path: &str) -> i64 {
    let mut graph = parse_file(path);

    let mut second = 0;

    let mut queue = VecDeque::new();

    for n in graph.neighbours(graph.start) {
        queue.push_back((*n, graph.start, *n, 1));
    }
    graph.mark(graph.start);
    while queue.len() > 0 {
        let (seed, prev, curr, depth) = queue.pop_front().unwrap();
        graph.mark(curr);
        for n in graph.neighbours(curr) {
            if *n == prev {
                continue;
            }

            if graph.nodes[*n].visited {
                second = seed;
                break;
            }

            queue.push_back((seed, curr, *n, depth + 1));
        }
    }

    let mut path = Vec::new();
    let mut curr = second;
    let mut prev = graph.start;
    path.push(graph.nodes[graph.start].clone());
    while curr != graph.start {
        let node = &graph.nodes[curr];
        path.push(node.clone());
        assert!(node.neighbours.len() == 2);
        // println!("{prev}->{curr} {:?}", node.neighbours);
        let mut neighbours = node.neighbours.iter().filter(|n| **n != prev);
        let next_idx = *neighbours.next().unwrap();
        assert!(neighbours.next().is_none());

        prev = curr;
        curr = next_idx;
    }
    graph.solve_start_symbol(second, prev);
    path[0].symbol = graph.nodes[graph.start].symbol;

    let mut grid: Vec<Vec<Option<char>>> = Vec::new();
    for y in 0..graph.y_dim {
        let mut row = Vec::new();
        for x in 0..graph.x_dim {
            row.push(None);
        }
        grid.push(row);
    }
    for p in path {
        grid[p.y][p.x] = Some(p.symbol);
    }
    let mut total = 0;
    let mut state = FillState::Out;
    for y in 0..graph.y_dim {
        for x in 0..graph.x_dim {
            match (grid[y][x], state) {
                (None, FillState::In) => {
                    total += 1;
                }
                (None, FillState::Out) => { /* NOOP */ }
                (Some('|'), FillState::Out) => state = FillState::In,
                (Some('|'), FillState::In) => state = FillState::Out,
                (Some('-'), _) => {}
                (Some('L'), FillState::Out) => state = FillState::TopIn,
                (Some('L'), FillState::In) => state = FillState::BottomIn,
                (Some('F'), FillState::Out) => state = FillState::BottomIn,
                (Some('F'), FillState::In) => state = FillState::TopIn,
                (Some('7'), FillState::TopIn) => state = FillState::In,
                (Some('7'), FillState::BottomIn) => state = FillState::Out,
                (Some('J'), FillState::TopIn) => state = FillState::Out,
                (Some('J'), FillState::BottomIn) => state = FillState::In,
                _ => {
                    println!("{:?}{state:?}", grid[y][x]);
                    panic!("Dev Error");
                }
            }
        }
    }

    total
}

fn main() {
    let answer1 = part_1("./src/bin/day10/puzzle.txt");
    let answer2 = part_2("./src/bin/day10/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
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

#[test]
fn can_parse_part2_sample1() {
    let answer = part_2("./src/bin/day10/part2_sample1.txt");

    assert_eq!(answer, 4);
}
#[test]
fn can_parse_part2_sample2() {
    let answer = part_2("./src/bin/day10/part2_sample2.txt");

    assert_eq!(answer, 10);
}

#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day10/puzzle.txt");

    assert_eq!(answer, 541);
}
