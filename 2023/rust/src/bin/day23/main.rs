use std::{
    collections::{HashMap, HashSet},
    fs,
};

type GraphMap = HashMap<(usize, usize), Vec<(usize, usize)>>;

#[derive(Debug, PartialEq, Eq, Clone)]
struct Maze {
    graph: GraphMap,
    start: (usize, usize),
    width: usize,
    height: usize,
}

const DIRS: [(i64, i64); 4] = [(-1_i64, 0_i64), (1, 0), (0, -1), (0, 1)];

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
                '.' => DIRS
                    .iter()
                    .filter_map(|d| {
                        let candidate = (row as i64 + d.0, col as i64 + d.1);
                        if candidate.0 >= 0
                            && candidate.0 < height as i64
                            && candidate.1 >= 0
                            && candidate.1 < width as i64
                            && grid[candidate.0 as usize][candidate.1 as usize] != '#'
                        {
                            Some((candidate.0 as usize, candidate.1 as usize))
                        } else {
                            None
                        }
                    })
                    .collect(),
                '^' => vec![(row - 1, col)],
                'v' => vec![(row + 1, col)],
                '>' => vec![(row, col + 1)],
                '<' => vec![(row, col - 1)],
                _ => unreachable!(),
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
            for &next_pos in maze.graph.get(&pos).unwrap() {
                queue.push((score + 1, next_pos, seen.clone()));
            }
        }
    }
    result
}

fn part_2(path: &str) -> u64 {
    let maze = parse_file(path);
    0
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

// #[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day23/sample.txt");
    assert_eq!(answer, 0);
}

// #[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day23/puzzle.txt");

    assert_eq!(answer, 0);
}
