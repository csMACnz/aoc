use std::{
    collections::HashMap,
    fs,
};

struct DigPlan {
    work: Vec<Work>,
}

struct Work {
    direction: Direction,
    distance: isize,
    color: String,
}

impl Work {
    fn parse(s: &str) -> Work {
        let (lhs, rhs) = s.split_once(" (").unwrap();
        let (direction, distance) = lhs.split_once(" ").unwrap();
        let color = String::from(rhs.trim_end_matches(")"));
        let direction = match direction {
            "U" => Direction::Up,
            "D" => Direction::Down,
            "L" => Direction::Left,
            "R" => Direction::Right,
            _ => unreachable!(),
        };
        let distance = distance.parse::<isize>().unwrap();
        Work {
            direction,
            distance,
            color,
        }
    }
}

enum Direction {
    Up,
    Down,
    Left,
    Right,
}

fn parse_file(path: &str) -> DigPlan {
    let work = fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .lines()
        .map(|l| Work::parse(l))
        .collect();

    DigPlan { work }
}

fn calculate_area_using_shoelace(path: &Vec<(isize, isize)>) -> u64 {
    assert!(path[0] == path[path.len() - 1]);

    (path
        .windows(2)
        .map(|p| ((p[0].1 * p[1].0) as isize) - ((p[1].1 * p[0].0) as isize))
        .sum::<isize>() as usize
        / 2) as u64
}
fn calculate_area_using_in_out_odd_even(path: &Vec<(isize, isize)>) -> u64 {
    assert!(path[0] == path[path.len() - 1]);

    let min_x = path.iter().map(|(_, x)| x).min().unwrap();
    let max_x = path.iter().map(|(_, x)| x).max().unwrap();
    let min_y = path.iter().map(|(y, _)| y).min().unwrap();
    let max_y = path.iter().map(|(y, _)| y).max().unwrap();
    let path = path
        .iter()
        .map(|(y, x)| ((y - min_y) as usize, (x - min_x) as usize))
        .collect::<Vec<(usize, usize)>>();
    let width = (max_x - min_x + 1) as usize;
    let height = (max_y - min_y + 1) as usize;

    let mut grid = HashMap::new();
    for p in vec![path[path.len() - 2]]
        .iter()
        .chain(path.iter())
        .cloned()
        .collect::<Vec<(usize, usize)>>()
        .windows(3)
    {
        let corner = match p {
            [(y0, x0), (y1, x1), (y2, _)] if y0 == y1 && x0 < x1 && y2 > y1 => '7',
            [(y0, x0), (y1, x1), (y2, _)] if y0 == y1 && x0 > x1 && y2 > y1 => 'F',
            [(y0, x0), (y1, x1), (y2, _)] if y0 == y1 && x0 < x1 && y2 < y1 => 'J',
            [(y0, x0), (y1, x1), (y2, _)] if y0 == y1 && x0 > x1 && y2 < y1 => 'L',
            [(y0, x0), (y1, x1), (_, x2)] if x0 == x1 && y0 < y1 && x2 > x1 => 'L',
            [(y0, x0), (y1, x1), (_, x2)] if x0 == x1 && y0 > y1 && x2 > x1 => 'F',
            [(y0, x0), (y1, x1), (_, x2)] if x0 == x1 && y0 < y1 && x2 < x1 => 'J',
            [(y0, x0), (y1, x1), (_, x2)] if x0 == x1 && y0 > y1 && x2 < x1 => '7',
            _ => unreachable!(),
        };
        grid.insert(p[1], corner);
        if p[2].0 == p[1].0 {
            for x in usize::min(p[2].1, p[1].1) + 1..(usize::max(p[2].1, p[1].1)) {
                grid.insert((p[1].0, x), '-');
            }
        } else if p[2].1 == p[1].1 {
            for y in usize::min(p[2].0, p[1].0) + 1..(usize::max(p[2].0, p[1].0)) {
                grid.insert((y, p[1].1), '|');
            }
        } else {
            unreachable!();
        }
    }

    let mut ans = 0;
    for y in 0..height {
        for x in 0..width {
            if let Some(_) = grid.get(&(y, x)) {
                ans += 1;
            } else {
                if (1..=(y.min(x)))
                    .filter(|n| {
                        if let Some(&x) = grid.get(&(y - n, x - n)) {
                            x == '|' || x == '-' || x == 'F' || x == 'J'
                        } else {
                            false
                        }
                    })
                    .count()
                    % 2
                    == 1
                {
                    ans += 1;
                } else {
                }
            }
        }
    }
    ans
}

fn part_1(path: &str) -> u64 {
    let plan = parse_file(path);
    let mut path = Vec::new();
    let mut curr: (isize, isize) = (0, 0);
    path.push(curr);
    for work in &plan.work {
        let offset: (isize, isize) = match work.direction {
            Direction::Up => (-1, 0),
            Direction::Down => (1, 0),
            Direction::Left => (0, -1),
            Direction::Right => (0, 1),
        };
        curr = (
            curr.0 + offset.0 * work.distance,
            curr.1 + offset.1 * work.distance,
        );
        path.push(curr);
    }
    let a = calculate_area_using_in_out_odd_even(&path);
    let b = calculate_area_using_shoelace(&path)
        + (&plan.work.iter().map(|w| w.distance as u64).sum::<u64>() / 2)
        + 1;
    println!("{a:?}=={b:?}");
    assert!(a == b);
    b
}

fn part_2(path: &str) -> u64 {
    let plan = parse_file(path);

    let mut path = Vec::new();
    let mut perimeter = 0_u64;
    let mut curr: (isize, isize) = (0, 0);
    path.push(curr);
    //turn color into direction and distance
    for work in plan.work {
        // #70c710 = R 461937
        let offset = match work.color.split_at(&work.color.len() - 1).1 {
            "0" => (0_isize, 1_isize),
            "1" => (1, 0),
            "2" => (0, -1),
            "3" => (-1, 0),
            _ => unreachable!(),
        };
        let distance = isize::from_str_radix(&work.color[1..work.color.len() - 1], 16).unwrap();
        curr = (curr.0 + offset.0 * distance, curr.1 + offset.1 * distance);
        path.push(curr);
        perimeter += distance as u64;
    }

    calculate_area_using_shoelace(&path) + (perimeter / 2) + 1
}

fn main() {
    let answer1 = part_1("./src/bin/day18/puzzle.txt");
    let answer2 = part_2("./src/bin/day18/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample() {
    let answer = part_1("./src/bin/day18/sample.txt");

    assert_eq!(answer, 62);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day18/puzzle.txt");

    assert_eq!(answer, 44436);
}

#[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day18/sample.txt");

    assert_eq!(answer, 952408144115);
}

#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day18/puzzle.txt");

    assert_eq!(answer, 106941819907437);
}
