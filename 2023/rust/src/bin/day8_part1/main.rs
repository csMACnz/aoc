use std::{collections::HashMap, fs};

struct Map {
    directions: Vec<Direction>,
    graph: HashMap<String, (String, String)>,
}

#[derive(Copy, Clone)]
enum Direction {
    Left,
    Right,
}

fn parse_directions(directions: &str) -> Vec<Direction> {
    directions
        .chars()
        .map(|d| match d {
            'L' => Direction::Left,
            'R' => Direction::Right,
            _ => panic!("Only expecting L and R"),
        })
        .collect()
}

fn parse_file(path: &str) -> Map {
    let file = fs::read_to_string(path).expect("Should have been able to read the file");
    let mut lines = file.lines().into_iter();
    let directions = parse_directions(lines.next().unwrap());
    let mut graph = HashMap::new();
    lines.next().expect("expect empty line to skip");
    for line in lines {
        let (key, lr) = line.split_once(" = ").unwrap();
        let (left, right) = lr[1..lr.len()-1].split_once(", ").unwrap();
        graph.insert(key.to_owned(), (left.to_owned(), right.to_owned()));
    }
    Map { directions, graph }
}

fn part_1(path: &str) -> u64 {
    let map = parse_file(path);

    let mut index = 0;
    let mut current = &map.graph["AAA"];
    let mut count = 0;
    loop {
        let direction = map.directions[index];
        let next = match direction {
            Direction::Left => &current.0,
            Direction::Right => &current.1,
        };
        count +=1;
        if next == "ZZZ" {
            break;
        }
        current = &map.graph[next];

        index = (index + 1) % map.directions.len();
    }
    count
}

fn main() {
    let answer = part_1("./src/bin/day8_part1/puzzle.txt");

    println!("Answer: {}", answer);
}

#[test]
fn can_parse_sample_1() {
    let answer = part_1("./src/bin/day8_part1/sample.txt");

    assert_eq!(answer, 2);
}

#[test]
fn can_parse_sample_2() {
    let answer = part_1("./src/bin/day8_part1/sample2.txt");

    assert_eq!(answer, 6);
}


#[test]
fn can_parse_puzzle() {
    let answer = part_1("./src/bin/day8_part1/puzzle.txt");

    assert_eq!(answer, 15989);
}
