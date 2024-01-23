use std::{
    collections::{HashMap, HashSet},
    fs,
    str::FromStr,
};

type Position = (i64, i64, i64);
type Velocity = (i64, i64, i64);
struct Stone {
    position: Position,
    velocity: Velocity,
}

// https://www.geeksforgeeks.org/program-for-point-of-intersection-of-two-lines/
fn line_line_intersection(
    a: (f64, f64),
    b: (f64, f64),
    c: (f64, f64),
    d: (f64, f64),
) -> Option<(f64, f64)> {
    // Line AB represented as a1x + b1y = c1
    let a1 = b.1 - a.1;
    let b1 = a.0 - b.0;
    let c1 = a1 * (a.0) + b1 * (a.1);

    // Line CD represented as a2x + b2y = c2
    let a2 = d.1 - c.1;
    let b2 = c.0 - d.0;
    let c2 = a2 * (c.0) + b2 * (c.1);

    let determinant = a1 * b2 - a2 * b1;

    if determinant == 0.0 {
        // The lines are parallel. This is simplified
        // by returning a pair of FLT_MAX
        None
    } else {
        let x = (b2 * c1 - b1 * c2) / determinant;
        let y = (a1 * c2 - a2 * c1) / determinant;
        Some((x, y))
    }
}

impl Stone {
    fn intersects_xy_with(&self, other: &Self) -> Option<(f64, f64)> {
        line_line_intersection(
            (self.position.0 as f64, self.position.1 as f64),
            (
                self.position.0 as f64 + self.velocity.0 as f64,
                self.position.1 as f64 + self.velocity.1 as f64,
            ),
            (other.position.0 as f64, other.position.1 as f64),
            (
                other.position.0 as f64 + other.velocity.0 as f64,
                other.position.1 as f64 + other.velocity.1 as f64,
            ),
        )
    }
}

impl FromStr for Stone {
    type Err = String;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let (pos, vel) = s.split_once(" @ ").unwrap();
        let pos = three_split_num(pos);
        let vel = three_split_num(vel);
        Ok(Stone {
            position: pos,
            velocity: vel,
        })
    }
}

fn three_split_num(pos: &str) -> (i64, i64, i64) {
    let nums: [i64; 3] = pos
        .split(", ")
        .map(|s| s.trim().parse::<i64>().unwrap())
        .collect::<Vec<_>>()
        .try_into()
        .unwrap();
    (nums[0], nums[1], nums[2])
}

fn parse_file(path: &str) -> Vec<Stone> {
    fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .lines()
        .map(|l| l.parse().unwrap())
        .collect()
}

fn part_1(path: &str, test_area_min: i64, test_area_max: i64) -> u64 {
    let stones = parse_file(path);
    let mut count = 0;
    for i in 0..stones.len() {
        for j in (i + 1)..stones.len() {
            let a = &stones[i];
            let b = &stones[j];
            if let Some((x, y)) = a.intersects_xy_with(&b) {
                // y' = py + vy * t
                // t = (y' - py) / vy
                let tax = (x - a.position.0 as f64) / (a.velocity.0 as f64);
                let tbx = (x - b.position.0 as f64) / (b.velocity.0 as f64);

                if tax >= 0_f64 && tbx >= 0_f64 {
                    // must be forward in time
                    if x >= test_area_min as f64
                        && x <= test_area_max as f64
                        && y >= test_area_min as f64
                        && y <= test_area_max as f64
                    {
                        // must be in bounds
                        count += 1;
                    }
                }
            }
        }
    }
    count
}

fn part_2(path: &str) -> u64 {
    let stones = parse_file(path);
    0
}

fn main() {
    let answer1 = part_1(
        "./src/bin/day24/puzzle.txt",
        200000000000000,
        400000000000000,
    );
    let answer2 = part_2("./src/bin/day24/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample() {
    let answer = part_1("./src/bin/day24/sample.txt", 7, 27);

    assert_eq!(answer, 2);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1(
        "./src/bin/day24/puzzle.txt",
        200000000000000,
        400000000000000,
    );

    assert_ne!(answer, 19022);
    assert_eq!(answer, 26657);
}

// #[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day24/sample.txt");
    assert_eq!(answer, 0);
}

// #[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day24/puzzle.txt");

    assert_eq!(answer, 0);
}
