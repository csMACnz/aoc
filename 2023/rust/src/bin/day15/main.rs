use std::{collections::HashMap, fs};

struct Chunk {
    part: String,
}

fn hash(s: &str) -> u64 {
    s.chars().fold(0, |acc, x| ((acc + (x as u64)) * 17) % 256)
}

impl Chunk {
    fn hash_chunk(&self) -> u64 {
        hash(&self.part)
    }

    fn parts(&self) -> (&str, Option<u8>) {
        if self.part.ends_with('-') {
            (&self.part[0..self.part.len() - 1], None)
        } else {
            let x = self.part.split_once('=').unwrap();
            (x.0, x.1.parse().ok())
        }
    }
}

fn parse(chunk: &str) -> Chunk {
    Chunk {
        part: String::from(chunk),
    }
}

fn parse_file(path: &str) -> Vec<Chunk> {
    fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .split(',')
        .map(parse)
        .collect()
}

fn part_1(path: &str) -> u64 {
    let chunks = parse_file(path);
    chunks.iter().map(|c| c.hash_chunk()).sum()
}

fn part_2(path: &str) -> u64 {
    let chunks = parse_file(path);
    let mut boxes: HashMap<u64, Vec<(&str, u8)>> = HashMap::new();
    for chunk in chunks.iter() {
        let (label, focal_length) = chunk.parts();
        let hash = hash(&label);
        if let Some(bucket) = boxes.get_mut(&hash) {
            if let Some(idx) = bucket.iter().position(|&b| b.0 == label) {
                if let Some(focal_length) = focal_length {
                    bucket[idx].1 = focal_length;
                } else {
                    bucket.remove(idx);
                }
            } else {
                if let Some(focal_length) = focal_length {
                    bucket.push((label, focal_length));
                }
            }
        } else {
            if let Some(focal_length) = focal_length {
                boxes.insert(hash, vec![(label, focal_length)]);
            }
        }
    }
    boxes
        .iter()
        .map(|(k, lenses)| {
            lenses
                .iter()
                .enumerate()
                .map(|(i, (_, f))| (1 + *k) * (1 + i as u64) * (*f as u64))
                .sum::<u64>()
        })
        .sum()
}

fn main() {
    let answer1 = part_1("./src/bin/day15/puzzle.txt");
    let answer2 = part_2("./src/bin/day15/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample() {
    let answer = part_1("./src/bin/day15/sample.txt");

    assert_eq!(answer, 1320);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day15/puzzle.txt");

    assert_eq!(answer, 517315);
}

#[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day15/sample.txt");

    assert_eq!(answer, 145);
}

#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day15/puzzle.txt");

    assert_eq!(answer, 247763);
}
