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

fn bound_offset(n: i64, dim: usize) -> usize {
    let result = if n < 0 {
        dim - (n.abs() as usize % dim)
    } else {
        n as usize % dim
    };
    result
}

fn part_2(path: &str, distance: u64) -> u64 {
    let grid = parse_file(path);
    let mut prev_day: HashSet<(i64, i64)> = HashSet::new();
    prev_day.insert((grid.start.0 as i64, grid.start.1 as i64));

    let (modulo, iteration_count) = if grid.width < 32 {
        (grid.width as u64, 10)
    } else {
        (grid.width as u64, 4)
    };
    let offset = distance % modulo;
    let x = (distance - offset) / modulo;
    assert_eq!(distance, x * modulo + offset);

    let mut deltas = Vec::new();
    for i in 1..=distance {
        let mut next_day = HashSet::new();
        for p in &prev_day {
            for (y_offset, x_offset) in vec![(-1_i64, 0_i64), (1, 0), (0, -1), (0, 1)] {
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
        let next_day_len = next_day.len() as u64;
        if ((i - 1) - offset) % modulo == 0
            || ((i) - offset) % modulo == 0
            || ((i + 1) - offset) % modulo == 0
        {
            println!("{}=>{}", i, next_day_len);
        }
        if i >= offset && deltas.len() < iteration_count {
            if (i - offset) % modulo == 0 {
                if deltas.len() == 0 {
                    deltas.push((i, next_day_len, 0_u64, 0_u64));
                } else {
                    let (_, prev_len, last_delta, _) = deltas[deltas.len() - 1];
                    let delta = next_day_len - prev_len;
                    let second_derivative = if last_delta == 0 {
                        0
                    } else {
                        delta - last_delta
                    };

                    deltas.push((i, next_day_len, delta, second_derivative));

                    if deltas.len() >= iteration_count {
                        // panic!();
                        //solve

                        println!("{:?}", deltas);
                        // let (quad_offset, _) = deltas
                        //     .windows(2)
                        //     .enumerate()
                        //     .filter(|(i, [a, b])| a.3 == b.3)
                        //     .take(1)
                        //     .next()
                        //     .unwrap();

                        assert!(deltas[deltas.len() - 2].3 == deltas[deltas.len() - 1].3);
                        let a = deltas[deltas.len() - 1].3 as u64 / 2;
                        let c = deltas[deltas.len() - 4].1 as u64;
                        let b = deltas[deltas.len() - 3].1 as u64 - a - c;
                        let x = x - (deltas.len() as u64 - 4);
                        // let bb = deltas[1].2 as u64 - (2 * a); // b = y' - 2ax
                        // assert_eq!(bb, b);

                        println!("x={}", x);
                        println!("a={} b={} c={}", a, b, c);
                        println!("0={}", a * 0 * 0 + b * 0 + c);
                        println!("1={}", a * 1 * 1 + b * 1 + c);
                        println!("2={}", a * 2 * 2 + b * 2 + c);
                        println!("3={}", a * 3 * 3 + b * 3 + c);
                        println!("4={}", a * 4 * 4 + b * 4 + c);
                        println!("5={}", a * 5 * 5 + b * 5 + c);

                        println!("(x-1)={}", a * (x - 1) * (x - 1) + b * (x - 1) + c);
                        println!("x={}", a * x * x + b * x + c);
                        println!("(x+1)={}", a * (x + 1) * (x + 1) + b * (x + 1) + c);
                        return a * x * x + b * x + c;
                    }
                }
            }
        }
        prev_day = next_day
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
