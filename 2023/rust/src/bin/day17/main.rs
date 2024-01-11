use std::cmp::Ordering;
use std::collections::BinaryHeap;
use std::{collections::HashMap, fs};

#[derive(Copy, Clone, Eq, PartialEq)]
struct State {
    cost: u64,
    position: Pos,
    incomming_direction: Direction,
    count: u64,
}

// The priority queue depends on `Ord`.
// Explicitly implement the trait so the queue becomes a min-heap
// instead of a max-heap.
impl Ord for State {
    fn cmp(&self, other: &Self) -> Ordering {
        // Notice that the we flip the ordering on costs.
        // In case of a tie we compare positions - this step is necessary
        // to make implementations of `PartialEq` and `Ord` consistent.
        other
            .cost
            .cmp(&self.cost)
            .then_with(|| self.position.cmp(&other.position))
            .then_with(|| self.count.cmp(&other.count))
    }
}

// `PartialOrd` needs to be implemented as well.
impl PartialOrd for State {
    fn partial_cmp(&self, other: &Self) -> Option<Ordering> {
        Some(self.cmp(other))
    }
}

#[derive(Debug, Copy, Clone, PartialEq, Eq, PartialOrd, Ord, Hash)]
struct Pos(usize, usize);

#[derive(Debug, Copy, Clone, PartialEq, Eq, Hash)]
enum Direction {
    Start,
    Left,
    Right,
    Up,
    Down,
}

impl Pos {
    fn mv(self: &Self, dir: Direction, width: usize, height: usize) -> Option<Pos> {
        match dir {
            Direction::Start => panic!(),
            Direction::Up if self.0 <= 0 => None,
            Direction::Left if self.1 <= 0 => None,
            Direction::Right if self.1 >= (width - 1) => None,
            Direction::Down if self.0 >= (height - 1) => None,
            Direction::Up => Some(Pos(self.0 - 1, self.1)),
            Direction::Left => Some(Pos(self.0, self.1 - 1)),
            Direction::Down => Some(Pos(self.0 + 1, self.1)),
            Direction::Right => Some(Pos(self.0, self.1 + 1)),
        }
    }
}

struct Grid {
    data: Vec<Vec<u64>>,
    width: usize,
    height: usize,
}

impl Grid {
    fn mv(self: &Self, pos: Pos, dir: Direction) -> Option<Pos> {
        pos.mv(dir, self.width, self.height)
    }
    fn at(self: &Self, pos: Pos) -> u64 {
        self.data[pos.0][pos.1]
    }
}

fn parse_file(path: &str) -> Grid {
    let data: Vec<Vec<u64>> = fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .lines()
        .map(|l| {
            l.chars()
                .map(|c| c.to_digit(10).expect("grid should contain only digits") as u64)
                .collect()
        })
        .collect();

    Grid {
        width: data[0].len(),
        height: data.len(),
        data,
    }
}

fn part_1(path: &str) -> u64 {
    let grid = parse_file(path);

    // dist[node] = current shortest distance from `start` to `node`
    let mut dist: HashMap<(Pos, Direction, u64), u64> = HashMap::new();

    let mut heap: BinaryHeap<State> = BinaryHeap::new();
    let end = Pos(grid.height - 1, grid.width - 1);
    heap.push(State {
        cost: 0,
        position: Pos(0, 0),
        incomming_direction: Direction::Right,
        count: 0,
    });
    while let Some(state) = heap.pop() {
        if state.position == end {
            return state.cost;
        };
        if let Some(&heat) = dist.get(&(state.position, state.incomming_direction, state.count)) {
            if heat <= state.cost {
                continue;
            }
        }
        dist.insert(
            (state.position, state.incomming_direction, state.count),
            state.cost,
        );
        let turns = match state.incomming_direction {
            Direction::Start => [
                (Direction::Right, 0),
                (Direction::Right, 0),
                (Direction::Down, 0),
            ],
            Direction::Down | Direction::Up => [
                (state.incomming_direction, state.count),
                (Direction::Right, 0),
                (Direction::Left, 0),
            ],
            Direction::Right | Direction::Left => [
                (state.incomming_direction, state.count),
                (Direction::Down, 0),
                (Direction::Up, 0),
            ],
        };
        for (next_dir, step) in turns {
            if let Some(next_pos) = grid.mv(state.position, next_dir) {
                if step < 3 {
                    heap.push(State {
                        cost: state.cost + grid.at(next_pos),
                        position: next_pos,
                        incomming_direction: next_dir,
                        count: step + 1,
                    });
                }
            }
        }
    }
    unreachable!();
}

fn part_2(path: &str) -> u64 {
    let grid = parse_file(path);

    // dist[node] = current shortest distance from `start` to `node`
    let mut dist: HashMap<(Pos, Direction, u64), u64> = HashMap::new();

    let mut heap: BinaryHeap<State> = BinaryHeap::new();
    let end = Pos(grid.height - 1, grid.width - 1);
    heap.push(State {
        cost: 0,
        position: Pos(0, 0),
        incomming_direction: Direction::Start,
        count: 0,
    });
    while let Some(state) = heap.pop() {
        if state.position == end {
            return state.cost;
        };
        if let Some(&heat) = dist.get(&(state.position, state.incomming_direction, state.count)) {
            if heat <= state.cost {
                continue;
            }
        }
        dist.insert(
            (state.position, state.incomming_direction, state.count),
            state.cost,
        );
        let turns = match state.incomming_direction {
            Direction::Start => [
                (Direction::Start, 0),
                (Direction::Right, 0),
                (Direction::Down, 0),
            ],
            Direction::Down | Direction::Up => [
                (state.incomming_direction, state.count),
                (Direction::Right, 0),
                (Direction::Left, 0),
            ],
            Direction::Right | Direction::Left => [
                (state.incomming_direction, state.count),
                (Direction::Down, 0),
                (Direction::Up, 0),
            ],
        };
        for (next_dir, step) in turns {
            if next_dir != Direction::Start {
                if let Some(next_pos) = grid.mv(state.position, next_dir) {
                    let can_push = if next_dir == state.incomming_direction {
                        state.count < 10
                    } else {
                        if state.incomming_direction == Direction::Start {
                            true
                        }
                        else if state.count < 4 {
                            false
                        } else {
                            match next_dir {
                                Direction::Start => panic!(),
                                Direction::Left => state.position.1 >= 4,
                                Direction::Right => state.position.1 <= grid.width - 1 - 4,
                                Direction::Up => state.position.0 >= 4,
                                Direction::Down => state.position.0 <= grid.height - 1 - 4,
                            }
                        }
                    };
                    if can_push {
                        heap.push(State {
                            cost: state.cost + grid.at(next_pos),
                            position: next_pos,
                            incomming_direction: next_dir,
                            count: step + 1,
                        });
                    }
                }
            }
        }
    }
    unreachable!();
}

fn main() {
    let answer1 = part_1("./src/bin/day17/puzzle.txt");
    let answer2 = part_2("./src/bin/day17/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample() {
    let answer = part_1("./src/bin/day17/sample.txt");

    assert_eq!(answer, 102);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day17/puzzle.txt");

    assert_ne!(answer, 1074);
    assert_eq!(answer, 1110);
}

#[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day17/sample.txt");

    assert_eq!(answer, 94);
}

#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day17/puzzle.txt");

    assert_ne!(answer, 1304);
    assert_ne!(answer, 1303);
    assert_eq!(answer, 0);
}
