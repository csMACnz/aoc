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

fn lcm(n1: u64, n2: u64) -> u64 {
    let mut x;
    let mut y;
    let mut rem;

    if n1 > n2 {
        x = n1;
        y = n2;
    }
    else {
        x = n2;
        y = n1;
    }

    rem = x % y;

    while rem != 0 {
        x = y;
        y = rem;
        rem = x % y;
    }
    let lcm = n1 * n2 / y;
    lcm
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
        let (left, right) = lr[1..lr.len() - 1].split_once(", ").unwrap();
        graph.insert(key.to_owned(), (left.to_owned(), right.to_owned()));
    }
    Map { directions, graph }
}

fn calculate_loop<'a>(map: &'a Map, start_key: &String) -> (u64, &'a String) {
    let mut index = 0;
    let mut current = &map.graph[start_key];
    let mut count = 0;
    loop {
        let direction = map.directions[index];
        let next = match direction {
            Direction::Left => &current.0,
            Direction::Right => &current.1,
        };
        count += 1;
        if next.ends_with('Z') {
            return (count, next);
        }
        current = &map.graph[next];

        index = (index + 1) % map.directions.len();
    }
}

fn part_2(path: &str) -> u64 {
    let map = parse_file(path);
    let mut data = Vec::new();
    for start_key in map.graph.keys().filter(|x| x.ends_with('A')) {
        let (offset, end_key) = calculate_loop(&map, start_key);
        let (loop_num, _) = calculate_loop(&map, end_key);
        if offset != loop_num {
            panic!("aoc puzzle is proposed to have first offset == loop size");
        }
        data.push(loop_num);
    }
    println!("{:?}", data);

    data.into_iter()
        .reduce(|acc, n| lcm(acc, n))
        .expect("data is not empty")
}

fn main() {
    let answer = part_2("./src/bin/day8_part2/puzzle.txt");

    println!("Answer: {}", answer);
}

#[test]
fn can_parse_sample() {
    let answer = part_2("./src/bin/day8_part2/sample.txt");

    assert_eq!(answer, 6);
}

#[test]
fn can_parse_puzzle() {
    let answer = part_2("./src/bin/day8_part2/puzzle.txt");

    assert_eq!(answer, 13830919117339);
}
