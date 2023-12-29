use std::{collections::HashMap, fs};
type Cache = HashMap<(String, Option<u64>, Vec<u64>), u64>;

#[derive(Clone)]
struct Spring {
    state: String,
    counts: Vec<u64>,
}

fn parse_file(path: &str) -> Vec<Spring> {
    fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .lines()
        .map(|l| {
            let (springs, counts) = l.split_once(" ").expect("aoc format '???.### 1,1,3");
            Spring {
                state: springs.to_owned(),
                counts: counts
                    .split(",")
                    .map(|c| c.parse().expect("aoc format '1,1,3'"))
                    .collect(),
            }
        })
        .collect()
}

fn count_arrangements_cache(
    state: &str,
    current: Option<u64>,
    remainder_counts: &[u64],
    cache: &mut Cache,
) -> u64 {
    let key = (state.to_owned(), current, remainder_counts.to_vec());
    if let Some(result) = cache.get(&key) {
        return *result;
    }
    let result = count_arrangements(state, current, remainder_counts, cache);
    cache.insert(key, result);
    result
}

fn count_arrangements(
    state: &str,
    current: Option<u64>,
    remainder_counts: &[u64],
    cache: &mut Cache,
) -> u64 {
    if state.is_empty() {
        if !remainder_counts.is_empty() || current.is_some_and(|x| x > 0) {
            0
        } else {
            1
        }
    } else {
        if state.starts_with('.') {
            if current.is_none() || current == Some(0) {
                count_arrangements_cache(&state[1..], None, remainder_counts, cache)
            } else {
                0
            }
        } else if state.starts_with('?') {
            let mut state_if_damaged = state.to_owned();
            let mut state_if_operational = state.to_owned();
            state_if_damaged.replace_range(0..1, "#");
            state_if_operational.replace_range(0..1, ".");
            count_arrangements_cache(&state_if_damaged, current, &remainder_counts, cache)
                + count_arrangements_cache(&state_if_operational, current, &remainder_counts, cache)
        } else if state.starts_with('#') {
            if current == Some(0) {
                0
            } else if current.is_none() && remainder_counts.is_empty() {
                0
            } else if current.is_none() {
                let (new_current, new_remainder) = remainder_counts.split_first().unwrap();
                count_arrangements_cache(&state[1..], Some(new_current - 1), &new_remainder, cache)
            } else if let Some(x) = current {
                count_arrangements_cache(&state[1..], Some(x - 1), remainder_counts, cache)
            } else {
                unreachable!();
            }
        } else {
            unreachable!();
        }
    }
}

fn part_1(path: &str) -> u64 {
    let springs = parse_file(path);
    let mut cache = HashMap::<(String, Option<u64>, Vec<u64>), u64>::new();
    springs
        .iter()
        .map(|spring| count_arrangements_cache(&spring.state, None, &spring.counts, &mut cache))
        .sum()
}

fn part_2(path: &str) -> u64 {
    let springs = parse_file(path);

    let mut cache = HashMap::<(String, Option<u64>, Vec<u64>), u64>::new();
    springs
        .into_iter()
        .map(|s| -> Spring {
            let counts_len = s.counts.len();
            let state = vec![s.state; 5].join("?").to_string();
            Spring {
                state: state,
                counts: s.counts.into_iter().cycle().take(counts_len * 5).collect(),
            }
        })
        .map(|spring| count_arrangements_cache(&spring.state, None, &spring.counts, &mut cache))
        .sum()
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

#[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day12/part1_sample.txt");

    assert_eq!(answer, 525152);
}


#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day12/puzzle.txt");

    assert_eq!(answer, 37366887898686);
}