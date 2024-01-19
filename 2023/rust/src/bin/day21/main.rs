use std::{
    collections::HashSet,
    fs,
};

struct Map {
    width: usize,
    height: usize,
    rocks: HashSet<(usize, usize)>,
    start: (usize, usize),
}

fn parse_file(path: &str) -> Map {
    let mut height = 0;
    let mut width = 0;
    let mut rocks = HashSet::new();
    let mut start = (0, 0);
    for (y, line) in fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .lines()
        .enumerate()
    {
        width = line.len();
        height += 1;
        for (x, c) in line.chars().enumerate() {
            match c {
                'S' => {
                    start = (y, x);
                }
                '#' => {
                    rocks.insert((y, x));
                }
                _ => {}
            };
        }
    }
    Map {
        width,
        height,
        rocks,
        start,
    }
}

fn part_1(path: &str, distance: u64) -> u64 {
    let grid = parse_file(path);
    let mut prev_day = HashSet::new();
    prev_day.insert(grid.start);

    for i in 0..distance {
        let mut next_day = HashSet::new();
        for p in prev_day {
            for (y_offset, x_offset) in vec![(-1_isize, 0_isize), (1, 0), (0, -1), (0, 1)] {
                match (
                    p.0.checked_add_signed(y_offset),
                    p.1.checked_add_signed(x_offset),
                ) {
                    (Some(y), Some(x)) if y < grid.height && x < grid.width => {
                        if !grid.rocks.contains(&(y, x)) {
                            next_day.insert((y, x));
                        }
                    }
                    _ => {}
                }
            }
        }
        prev_day = next_day
    }
    prev_day.len() as u64
}

fn part_2(path: &str) -> u64 {
    let grid = parse_file(path);
    0
}

fn main() {
    let answer1 = part_1("./src/bin/day21/puzzle.txt", 64);
    let answer2 = part_2("./src/bin/day21/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample_1() {
    let answer = part_1("./src/bin/day21/sample.txt", 6);

    assert_eq!(answer, 16);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day21/puzzle.txt", 64);

    assert_eq!(answer, 3841);
}

// #[test]
fn can_parse_part2_sample_2() {
    let answer = part_2("./src/bin/day21/sample.txt");
    assert_eq!(answer, 0);
}

// #[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day21/puzzle.txt");

    assert_eq!(answer, 0);
}
