use std::{
    collections::{HashSet, VecDeque},
    fs,
};

fn parse_file(path: &str) -> Vec<Vec<char>> {
    fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .lines()
        .map(|l| l.chars().collect())
        .collect()
}

fn energise_tiles(grid: &Vec<Vec<char>>, start: (isize, isize), dir: (i8, i8)) -> u64 {
    let mut seen = HashSet::new();
    let mut result = HashSet::new();
    let mut queue: VecDeque<((isize, isize), (i8, i8))> = VecDeque::new();
    queue.push_back((start, dir));
    while let Some((last, dir)) = queue.pop_front() {
        if (last.0 == 0 && dir.0 == -1)
            || (last.0 == (grid.len() - 1) as isize && dir.0 == 1)
            || (last.1 == 0 && dir.1 == -1)
            || (last.1 == (grid[0].len() - 1) as isize && dir.1 == 1)
        {
            continue;
        };
        let next = (last.0 + dir.0 as isize, last.1 + dir.1 as isize);
        // println!("({},{})", next.0, next.1);
        result.insert(next);
        if seen.insert((next, dir)) {
            match grid[next.0 as usize][next.1 as usize] {
                '.' => {
                    queue.push_back((next, dir));
                }
                '\\' => queue.push_back((next, (dir.1, dir.0))),
                '/' => queue.push_back((next, (-dir.1, -dir.0))),
                '|' => {
                    if dir.0 == 0 {
                        queue.push_back((next, (1, 0)));
                        queue.push_back((next, (-1, 0)));
                    } else {
                        queue.push_back((next, dir));
                    }
                }
                '-' => {
                    if dir.1 == 0 {
                        queue.push_back((next, (0, 1)));
                        queue.push_back((next, (0, -1)));
                    } else {
                        queue.push_back((next, dir));
                    }
                }
                _ => {
                    unreachable!("expect only the above chars");
                }
            }
        }
    }
    // let seen: HashSet<(isize, isize)> = seen.iter().map(|(coords, _)| coords.clone()).collect();
    // for y in 0..grid.len() {
    //     for x in 0..grid[0].len() {
    //         print!("{}", if seen.contains(&(y, x)) { '#' } else { '.' });
    //     }
    //     println!();
    // }
    result.len() as u64
}

fn part_1(path: &str) -> u64 {
    let grid = parse_file(path);
    energise_tiles(&grid, (0, -1), (0, 1))
}

fn part_2(path: &str) -> u64 {
    let grid = parse_file(path);
    let mut max = 0;
    for y in 0..grid.len() {
        max = max.max(energise_tiles(&grid, (y as isize, -1), (0, 1)));
        max = max.max(energise_tiles(&grid, (y as isize, grid[0].len() as isize), (0, -1)));
    }
    for x in 0..grid[0].len() {
        max = max.max(energise_tiles(&grid, (-1, x as isize), (1, 0)));
        max = max.max(energise_tiles(&grid, (grid.len() as isize, x as isize), (-1, 0)));
    }
    max
}

fn main() {
    let answer1 = part_1("./src/bin/day16/puzzle.txt");
    let answer2 = part_2("./src/bin/day16/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample() {
    let answer = part_1("./src/bin/day16/sample.txt");

    assert_eq!(answer, 46);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day16/puzzle.txt");

    assert_ne!(answer, 7440);
    assert_eq!(answer, 7498);
}

#[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day16/sample.txt");

    assert_eq!(answer, 51);
}

#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day16/puzzle.txt");

    assert_eq!(answer, 7846);
}
