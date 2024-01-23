use std::{
    collections::{HashMap, HashSet},
    fs,
};

#[derive(Debug, PartialEq, Eq, Clone)]
struct Tower {
    bricks: HashMap<usize, Brick>,
}

impl Tower {
    fn brick_drop(self: &mut Self) -> Vec<usize> {
        let mut to_fall = Vec::new();

        let mut sorted_bricks = self.bricks.iter().collect::<Vec<_>>();
        sorted_bricks.sort_by_key(|(_,b)|b.end.2); //sorted by end (bigger Z value);
        for index in 0..sorted_bricks.len() {
            let (&i, brick) = sorted_bricks[index];
            let mut can_fall = true;
            'cube_check: for cube in brick.cubes() {
                if cube.2 == 1 {
                    can_fall = false;
                    break;
                }
                //check below, until too far below
                for other_index in (0..index).rev() {
                    let  (_, other_brick) = sorted_bricks[other_index];
                    if brick != other_brick {
                        if other_brick.end.2 < brick.start.2-1 {
                            break;
                        }
                        if other_brick.is_below_cube(&cube) {
                            can_fall = false;
                            break 'cube_check;
                        }
                    }
                }
                //check above, until too far above
                for other_index in index+1..sorted_bricks.len() {
                    let  (_, other_brick) = sorted_bricks[other_index];
                    if brick != other_brick {
                        if other_brick.end.2 > brick.start.2 {
                            break;
                        }
                        if other_brick.is_below_cube(&cube) {
                            can_fall = false;
                            break 'cube_check;
                        }
                    }
                }
            }
            if can_fall {
                to_fall.push(i);
            }
        }
        if !to_fall.is_empty() {
            for (i, brick) in self.bricks.iter_mut() {
                if to_fall.contains(&i) {
                    brick.fall();
                }
            }
        };
        to_fall
    }

    fn apply_gravity(self: &mut Self) -> u64 {
        let mut fell = HashSet::new();
        loop {
            let fell_ids = self.brick_drop();
            if fell_ids.is_empty() {
                break fell.len() as u64;
            } else {
                for id in fell_ids {
                    fell.insert(id);
                }
            }
        }
    }

    fn apply_gravity_until_all_fall_at_least_once(self: &mut Self) -> u64 {
        let mut fell = HashSet::with_capacity(self.bricks.len());
        'result_loop: loop {
            let fell_ids = self.brick_drop();
            if fell_ids.is_empty() {
                break fell.len() as u64;
            } else {
                for id in fell_ids {
                    fell.insert(id);
                }
                if fell.len() == self.bricks.len() {
                    // break early
                    break 'result_loop fell.len() as u64;
                }
            }
        }
    }

    fn pull(&self, i: usize) -> Tower {
        let mut result = self.clone();
        result.bricks.remove_entry(&i);
        result
    }
}

impl From<String> for Tower {
    fn from(value: String) -> Self {
        Tower {
            bricks: value
                .lines()
                .enumerate()
                .map(|(i, l)| (i, l.into()))
                .collect(),
        }
    }
}

#[derive(Debug, PartialEq, Eq, Clone)]
struct Brick {
    start: (u64, u64, u64),
    end: (u64, u64, u64),
    cubes: Vec<(u64, u64, u64)>,
}

impl Brick {
    fn fall(&mut self) {
        self.start = (self.start.0, self.start.1, self.start.2 - 1);
        self.end = (self.end.0, self.end.1, self.end.2 - 1);

        for cube in &mut self.cubes {
            cube.2 -= 1;
        }
    }

    fn cubes(&self) -> &Vec<(u64, u64, u64)> {
        &self.cubes
    }

    fn is_below_cube(&self, cube: &(u64, u64, u64)) -> bool {
        self.start.2 <= (cube.2 - 1)
            && self.end.2 >= (cube.2 - 1)
            && self.start.1 <= cube.1
            && self.end.1 >= cube.1
            && self.start.0 <= cube.0
            && self.end.0 >= cube.0
    }
}

impl From<&str> for Brick {
    fn from(value: &str) -> Self {
        let (start, end) = value.split_once("~").unwrap();
        let start: [u64; 3] = start
            .split(',')
            .map(|c| c.parse::<u64>().unwrap())
            .collect::<Vec<u64>>()
            .try_into()
            .unwrap();
        let end: [u64; 3] = end
            .split(',')
            .map(|c| c.parse::<u64>().unwrap())
            .collect::<Vec<u64>>()
            .try_into()
            .unwrap();
        let cubes = if start[0] != end[0] {
            (start[0]..=end[0])
                .map(|x| (x, start[1], start[2]))
                .collect()
        } else if start[1] != end[1] {
            (start[1]..=end[1])
                .map(|y| (start[0], y, start[2]))
                .collect()
        } else if start[2] != end[2] {
            (start[2]..=end[2])
                .map(|z| (start[0], start[1], z))
                .collect()
        } else if start == end {
            vec![(start[0], start[1], start[2])]
        } else {
            unreachable!()
        };

        Brick {
            start: (start[0], start[1], start[2]),
            end: (end[0], end[1], end[2]),
            cubes,
        }
    }
}

fn parse_file(path: &str) -> Tower {
    fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .into()
}

fn part_1(path: &str) -> u64 {
    let mut tower = parse_file(path);
    tower.apply_gravity();
    let mut count = 0;
    for i in 0..tower.bricks.len() {
        let mut base_line = tower.pull(i);
        let none_fell = base_line.brick_drop().is_empty();
        if none_fell {
            count += 1;
        }
    }
    count
}

fn part_2(path: &str) -> u64 {
    let mut tower = parse_file(path);
    tower.apply_gravity();
    let mut count = 0;
    for i in 0..tower.bricks.len() {
        let mut base_line = tower.pull(i);
        let fall_count = base_line.apply_gravity_until_all_fall_at_least_once();
        count += fall_count;
    }
    count
}

fn main() {
    let answer1 = part_1("./src/bin/day22/puzzle.txt");
    let answer2 = part_2("./src/bin/day22/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample() {
    let answer = part_1("./src/bin/day22/sample.txt");

    assert_eq!(answer, 5);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day22/puzzle.txt");

    assert_eq!(answer, 426);
}

#[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day22/sample.txt");
    assert_eq!(answer, 7);
}

#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day22/puzzle.txt");

    assert_eq!(answer, 61920);
}
