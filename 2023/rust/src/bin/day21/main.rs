use std::{collections::HashSet, fs, vec};

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

    for _ in 1..=distance {
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

fn bound_offset(n: i64, dim: usize) -> usize {
    n.rem_euclid(dim as  i64) as usize
}

fn part_2(path: &str, distance: u64) -> u64 {
    let grid = parse_file(path);
    let directions = vec![(-1_i64, 0_i64), (1, 0), (0, -1), (0, 1)];
    let mut prev_day: HashSet<(i64, i64)> = HashSet::new();
    prev_day.insert((grid.start.0 as i64, grid.start.1 as i64));
    let mut prev_value = prev_day.len() as u64;
    let mut prev_delta = 0;
    let mut prev_second_derivative = 0;
    let modulo = grid.width as u64;
    let offset = distance % modulo;

    for i in 1..=distance {
        let mut next_day = HashSet::new();
        for p in &prev_day {
            for (y_offset, x_offset) in &directions {
                let p: (i64, i64) = (p.0 + y_offset, p.1 + x_offset);
                let rock_offset_p: (usize, usize) = (
                    bound_offset(p.0, grid.height),
                    bound_offset(p.1, grid.width),
                );
                if !grid.rocks.contains(&rock_offset_p) {
                    next_day.insert(p);
                }
            }
        }
        if i >= offset && (i - offset) % modulo == 0 {
            let next_value = next_day.len() as u64;
            let next_delta = next_value - prev_value;
            let second_derivative = if prev_delta == 0 {
                0
            } else {
                next_delta - prev_delta
            };
            if second_derivative != 0 && prev_second_derivative == second_derivative {
                // ready to solve

                let base_line = ((i - offset) / modulo) - 1;
                let raw_x = (distance - offset) / modulo;
                let x = raw_x - base_line;

                let a = second_derivative / 2;
                let c = prev_value;
                let b = next_value - a - c;

                return a * x * x + b * x + c;
            }

            prev_value = next_value;
            prev_delta = next_delta;
            prev_second_derivative = second_derivative;
        }
        prev_day = next_day;
    }
    prev_day.len() as u64
}

fn main() {
    let answer1 = part_1("./src/bin/day21/puzzle.txt", 64);
    let answer2 = part_2("./src/bin/day21/puzzle.txt", 26501365);

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

#[test]
fn can_parse_part2_sample_6_steps() {
    let answer = part_2("./src/bin/day21/sample.txt", 6);
    assert_eq!(answer, 16);
}

#[test]
fn can_parse_part2_sample_10() {
    let answer = part_2("./src/bin/day21/sample.txt", 10);
    assert_eq!(answer, 50);
}

#[test]
fn can_parse_part2_sample_50() {
    let answer = part_2("./src/bin/day21/sample.txt", 50);
    assert_eq!(answer, 1594);
}

#[test]
fn can_parse_part2_sample_100() {
    let answer = part_2("./src/bin/day21/sample.txt", 100);
    assert_eq!(answer, 6536);
}

#[test]
fn can_parse_part2_sample_500() {
    let answer = part_2("./src/bin/day21/sample.txt", 500);
    assert_eq!(answer, 167004);
}

#[test]
fn can_parse_part2_sample_1000() {
    let answer = part_2("./src/bin/day21/sample.txt", 1000);
    assert_eq!(answer, 668697);
}

#[test]
fn can_parse_part2_sample_5000() {
    let answer = part_2("./src/bin/day21/sample.txt", 5000);
    assert_eq!(answer, 16733044);
}

#[test]
fn can_parse_part2_puzzle_test() {
    let answer = part_2("./src/bin/day21/puzzle.txt", 64);

    assert_eq!(answer, 3841);
}

#[test]
fn can_parse_part2_puzzle_test_64() {
    let answer = part_2("./src/bin/day21/puzzle.txt", 64);

    assert_eq!(answer, 3841);
}

#[test]
fn can_parse_part2_puzzle_test_65() {
    let answer = part_2("./src/bin/day21/puzzle.txt", 65);

    assert_eq!(answer, 3947);
}

#[test]
fn can_parse_part2_puzzle_test_196() {
    let answer = part_2("./src/bin/day21/puzzle.txt", 196);

    assert_eq!(answer, 35153);
}

#[test]
fn can_parse_part2_puzzle_test_327() {
    let answer = part_2("./src/bin/day21/puzzle.txt", 327);

    assert_eq!(answer, 97459);
}

#[test]
fn can_parse_part2_puzzle_test_458() {
    let answer = part_2("./src/bin/day21/puzzle.txt", 458);

    assert_eq!(answer, 190865);
}

#[test]
fn can_parse_part2_puzzle_test_589() {
    let answer = part_2("./src/bin/day21/puzzle.txt", 589);

    assert_eq!(answer, 315371);
}

#[test]
fn can_parse_part2_puzzle_test_720() {
    let answer = part_2("./src/bin/day21/puzzle.txt", 720);

    assert_eq!(answer, 470977);
}

#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day21/puzzle.txt", 26501365);

    assert_eq!(answer, 636391426712747);
}
