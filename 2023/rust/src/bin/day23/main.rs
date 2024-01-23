use std::{
    collections::{HashMap, HashSet},
    fs,
};

type Position = (usize, usize);
type GraphMap = HashMap<Position, Vec<(Position, bool)>>;

#[derive(Debug, PartialEq, Eq, Clone)]
struct Maze {
    graph: GraphMap,
    start: Position,
    width: usize,
    height: usize,
}

#[derive(Debug, Clone, Eq, PartialEq)]
struct State {
    score: u64,
    position: Position,
    seen: HashSet<Position>,
}

fn parse_file(path: &str) -> Maze {
    let grid: Vec<Vec<char>> = fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .lines()
        .map(|l| l.chars().collect())
        .collect();
    let width = grid[0].len();
    let height = grid.len();
    let start = grid[0]
        .iter()
        .enumerate()
        .filter_map(|(i, &c)| if c == '.' { Some((0_usize, i)) } else { None })
        .next()
        .unwrap();
    // graph? edges are the size of the distance?
    let mut graph: GraphMap = HashMap::new();
    // apple_sauce(width, height, &mut graph, prev, start);
    for row in 0..height {
        for col in 0..width {
            let connections = match grid[row][col] {
                '#' => Vec::new(),
                c => {
                    let mut dirs = Vec::new();
                    dirs.push(((-1_i64, 0_i64), c == '.' || c == '^'));
                    dirs.push(((1, 0), c == '.' || c == 'v'));
                    dirs.push(((0, -1), c == '.' || c == '<'));
                    dirs.push(((0, 1), c == '.' || c == '>'));
                    dirs.iter()
                        .filter_map(|&(offset, easy)| {
                            let candidate = (row as i64 + offset.0, col as i64 + offset.1);
                            if candidate.0 >= 0
                                && candidate.0 < height as i64
                                && candidate.1 >= 0
                                && candidate.1 < width as i64
                                && grid[candidate.0 as usize][candidate.1 as usize] != '#'
                            {
                                Some(((candidate.0 as usize, candidate.1 as usize), easy))
                            } else {
                                None
                            }
                        })
                        .collect()
                }
            };
            graph.insert((row, col), connections);
        }
    }

    Maze {
        start,
        graph,
        width,
        height,
    }
}

fn part_1(path: &str) -> u64 {
    let maze = parse_file(path);
    let mut queue = Vec::new();
    queue.push((0, maze.start, HashSet::new()));
    let mut result = 0;
    while let Some((score, pos, mut seen)) = queue.pop() {
        if pos.0 == maze.height - 1 {
            result = result.max(score);
        } else {
            if !seen.insert(pos) {
                // already been here
                continue;
            }
            for &(next_pos, easy) in maze.graph.get(&pos).unwrap() {
                if easy {
                    queue.push((score + 1, next_pos, seen.clone()));
                }
            }
        }
    }
    result
}

fn part_2(path: &str) -> u64 {
    let maze = parse_file(path);

    // collapse the graph
    let new_graph: HashMap<Position, Vec<(Position, u64)>> = maze
        .graph
        .iter()
        .filter_map(|(&p, e)| {
            if e.len() != 2 {
                let destinations = e
                    .iter()
                    .map(|(d, _)| {
                        let mut prev = p;
                        let mut current = *d;
                        let mut next = maze.graph.get(&current).unwrap();
                        let mut steps = 1;
                        while next.len() == 2 {
                            steps += 1;
                            let next_node = if next[0].0 == prev {
                                next[1].0
                            } else {
                                next[0].0
                            };
                            prev = current;
                            current = next_node;
                            next = maze.graph.get(&current).unwrap();
                        }
                        (current, steps)
                    })
                    .collect();
                Some((p, destinations))
            } else {
                None
            }
        })
        .collect();

    let mut heap = Vec::new();
    heap.push(State {
        score: 0,
        position: maze.start,
        seen: HashSet::new(),
    });

    let mut result = 0;
    while let Some(mut state) = heap.pop() {
        // println!("{:?}", state.seen.len());
        if state.position.0 == maze.height - 1 {
            result = result.max(state.score);
            continue;
        } else {
            // let mut seen = state.seen;
            if !state.seen.insert(state.position) {
                // already been here
                continue;
            }
            for &(next_pos, weight) in new_graph.get(&state.position).unwrap() {
                heap.push(State {
                    score: state.score + weight,
                    position: next_pos,
                    seen: state.seen.clone(),
                });
            }
        }
    }
    result
}

fn main() {
    let answer1 = part_1("./src/bin/day23/puzzle.txt");
    let answer2 = part_2("./src/bin/day23/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample() {
    let answer = part_1("./src/bin/day23/sample.txt");

    assert_eq!(answer, 94);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day23/puzzle.txt");

    assert_eq!(answer, 2278);
}

#[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day23/sample.txt");
    assert_eq!(answer, 154);
}

#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day23/puzzle.txt");

    assert_eq!(answer, 6734);
}
