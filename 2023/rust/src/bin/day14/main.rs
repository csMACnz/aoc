use std::{collections::HashMap, fs};

fn parse_file(path: &str) -> Vec<Vec<char>> {
    fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .lines()
        .map(|l| l.chars().collect())
        .collect()
}

fn calculate_load(grid: &Vec<Vec<char>>) -> u64 {
    grid.iter()
        .enumerate()
        .map(|(index, row)| {
            let cost = (grid.len() - index) as u64;
            row.iter().filter(|c| **c == 'O').count() as u64 * cost
        })
        .sum::<u64>()
}

fn tilt_north(grid: &mut Vec<Vec<char>>) {
    for y in 0..grid.len() {
        for x in 0..grid[0].len() {
            if grid[y][x] == '.' {
                for yd in y..grid.len() {
                    match grid[yd][x] {
                        '.' => continue,
                        '#' => break,
                        'O' => {
                            grid[yd][x] = '.';
                            grid[y][x] = 'O';
                            break;
                        }
                        _ => unreachable!(),
                    };
                }
            }
        }
    }
}

fn tilt_south(grid: &mut Vec<Vec<char>>) {
    for _y in 0..grid.len() {
        let y = grid.len() - 1 - _y;
        for x in 0..grid[0].len() {
            if grid[y][x] == '.' {
                for _yd in 0..y {
                    let yd = y - 1 - _yd;
                    match grid[yd][x] {
                        '.' => continue,
                        '#' => break,
                        'O' => {
                            grid[yd][x] = '.';
                            grid[y][x] = 'O';
                            break;
                        }
                        _ => unreachable!(),
                    };
                }
            }
        }
    }
}

fn tilt_west(grid: &mut Vec<Vec<char>>) {
    for x in 0..grid[0].len() {
        for y in 0..grid.len() {
            if grid[y][x] == '.' {
                for xd in x..grid[0].len() {
                    match grid[y][xd] {
                        '.' => continue,
                        '#' => break,
                        'O' => {
                            grid[y][xd] = '.';
                            grid[y][x] = 'O';
                            break;
                        }
                        _ => unreachable!(),
                    };
                }
            }
        }
    }
}

fn tilt_east(grid: &mut Vec<Vec<char>>) {
    for _x in 0..grid[0].len() {
        let x = grid[0].len() - 1 - _x;
        for y in 0..grid.len() {
            if grid[y][x] == '.' {
                for _xd in 0..x {
                    let xd = x - 1 - _xd;
                    match grid[y][xd] {
                        '.' => continue,
                        '#' => break,
                        'O' => {
                            grid[y][xd] = '.';
                            grid[y][x] = 'O';
                            break;
                        }
                        _ => unreachable!(),
                    };
                }
            }
        }
    }
}

fn cycle(grid: &mut Vec<Vec<char>>) {
    tilt_north(grid);
    tilt_west(grid);
    tilt_south(grid);
    tilt_east(grid);
}

fn part_1(path: &str) -> u64 {
    let mut grid = parse_file(path);
    tilt_north(&mut grid);

    calculate_load(&grid)
}

fn part_2(path: &str) -> u64 {
    let mut grid = parse_file(path);
    let mut iterations: HashMap<Vec<Vec<char>>, usize> = HashMap::new();
    let mut index = 0;
    let first = loop {
        if let Some(first) = iterations.get(&grid) {
            break first;
        }
        iterations.insert(grid.clone(), index);
        
        cycle(&mut grid);
        index += 1;
    };

    let final_index = (1000000000 - first) % (index - first) + first;
    let final_grid = iterations
        .into_iter()
        .find(|(_, v)| *v == final_index)
        .unwrap()
        .0;
    calculate_load(&final_grid)
}

fn main() {
    let answer1 = part_1("./src/bin/day14/puzzle.txt");
    let answer2 = part_2("./src/bin/day14/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample() {
    let answer = part_1("./src/bin/day14/sample.txt");

    assert_eq!(answer, 136);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day14/puzzle.txt");

    assert_eq!(answer, 105623);
}

#[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day14/sample.txt");

    assert_eq!(answer, 64);
}


#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day14/puzzle.txt");

    assert_eq!(answer, 98029);
}
