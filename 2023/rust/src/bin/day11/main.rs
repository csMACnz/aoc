use std::{cmp::min, fs};

struct Grid {
    stars: Vec<(usize, usize)>,
    width: usize,
    height: usize,
}

fn parse_file(path: &str) -> Grid {
    let file = fs::read_to_string(path).expect("Should have been able to read the file");

    let stars = file
        .lines()
        .enumerate()
        .flat_map(|(y, l)| {
            l.chars()
                .enumerate()
                .filter_map(move |(x, c)| if c == '#' { Some((y, x)) } else { None })
        })
        .collect();
    Grid {
        height: file.lines().count(),
        width: file.lines().next().unwrap().chars().count(),
        stars,
    }
}

fn range_lookup(a: usize, b: usize, blocked_lookup: &Vec<u64>) -> u64 {
    let first = min(a, b);
    let second = if first == a { b } else { a };
    (first..second).map(|n| blocked_lookup[n]).sum::<u64>()
}

fn calculate_distance(grid: &Grid, growth: u64) -> u64 {
    let x_lookup: Vec<_> = (0..grid.width)
        .map(|col| {
            if grid.stars.iter().any(|(_, x)| col == *x) {
                1_u64
            } else {
                growth
            }
        })
        .collect();
    let y_lookup: Vec<_> = (0..grid.height)
        .map(|row| {
            if grid.stars.iter().any(|(y, _)| row == *y) {
                1_u64
            } else {
                growth
            }
        })
        .collect();

    let mut sum = 0;
    for i in 0..grid.stars.len() {
        for j in i + 1..grid.stars.len() {
            let first = grid.stars[i];
            let second = grid.stars[j];
            let path: u64 = range_lookup(first.0, second.0, &y_lookup)
                + range_lookup(first.1, second.1, &x_lookup);
            sum += path;
        }
    }
    sum
}

fn part_1(path: &str) -> u64 {
    let grid = parse_file(path);
    calculate_distance(&grid, 2)
}

fn part_2(path: &str) -> u64 {
    let grid = parse_file(path);
    calculate_distance(&grid, 1000000)
}

fn main() {
    let answer1 = part_1("./src/bin/day11/puzzle.txt");
    let answer2 = part_2("./src/bin/day11/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample() {
    let answer = part_1("./src/bin/day11/part1_sample.txt");

    assert_eq!(answer, 374);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day11/puzzle.txt");

    assert_eq!(answer, 9543156);
}

#[test]
fn can_parse_part2_sample_growth_10() {
    let grid = parse_file("./src/bin/day11/part1_sample.txt");
    let answer = calculate_distance(&grid, 10);
    assert_eq!(answer, 1030);
}

#[test]
fn can_parse_part2_sample_growth_100() {
    let grid = parse_file("./src/bin/day11/part1_sample.txt");
    let answer = calculate_distance(&grid, 100);
    assert_eq!(answer, 8410);
}

#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day11/puzzle.txt");

    assert_eq!(answer, 625243292686);
}