use std::fs;

struct Spring {
    state: Vec<SpringState>,
    counts: Vec<u64>,
}

#[derive(PartialEq, Clone, Copy)]
enum SpringState {
    Operational,
    Damaged,
    Unknown,
}

fn parse_spring(springs: &str) -> Vec<SpringState> {
    springs
        .chars()
        .map(|s| -> SpringState {
            match s {
                '.' => SpringState::Operational,
                '#' => SpringState::Damaged,
                '?' => SpringState::Unknown,
                _ => unreachable!(),
            }
        })
        .collect()
}

fn parse_file(path: &str) -> Vec<Spring> {
    fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .lines()
        .map(|l| {
            let (springs, counts) = l.split_once(" ").expect("aoc format '???.### 1,1,3");
            Spring {
                state: parse_spring(springs),
                counts: counts
                    .split(",")
                    .map(|c| c.parse().expect("aoc format '1,1,3'"))
                    .collect(),
            }
        })
        .collect()
}

fn count_arrangements(state: &[SpringState], counts: &[u64], inside: bool) -> u64 {
    if state.is_empty() {
        if counts.is_empty() {
            1
        } else if counts.len() == 1 && counts[0] == 0 {
            1
        } else {
            0
        }
    } else {
        if state[0] == SpringState::Operational {
            if !inside || counts.is_empty() {
                count_arrangements(&state[1..], counts, false)
            } else if counts[0] == 0 {
                count_arrangements(&state[1..], &counts[1..], false)
            } else {
                0
            }
        } else if state[0] == SpringState::Unknown {
            let mut state_if_damaged: Vec<SpringState> = state.into();
            let mut state_if_operational: Vec<SpringState> = state.into();
            state_if_damaged[0] = SpringState::Damaged;
            state_if_operational[0] = SpringState::Operational;
            count_arrangements(&state_if_damaged, &counts, inside)
                + count_arrangements(&state_if_operational, &counts, inside)
        } else if state[0] == SpringState::Damaged && counts.len() == 0 {
            0
        } else if state[0] == SpringState::Damaged && counts[0] == 0 {
            0
        } else if state[0] == SpringState::Damaged && counts[0] != 0 {
            let mut x: Vec<u64> = counts.into();
            x[0] -= 1;
            count_arrangements(&state[1..], &x, true)
        } else {
            0
        }
    }
}

fn part_1(path: &str) -> u64 {
    let springs = parse_file(path);
    springs
        .into_iter()
        .map(|spring| count_arrangements(&spring.state, &spring.counts, false))
        .sum()
}

fn part_2(path: &str) -> u64 {
    let springs = parse_file(path);
    0
}

fn main() {
    let answer1 = part_1("./src/bin/day12/puzzle.txt");
    let answer2 = part_2("./src/bin/day12/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample() {
    let answer = part_1("./src/bin/day12/part1_sample.txt");

    assert_eq!(answer, 21);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day12/puzzle.txt");

    assert_eq!(answer, 7916);
}
