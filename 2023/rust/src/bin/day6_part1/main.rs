use std::{fs, iter::zip};
fn split_nums(line: &str) -> Vec<u64> {
    line.split_once(":")
        .unwrap()
        .1
        .split(" ")
        .filter(|x| !x.is_empty())
        .map(|x| x.parse::<u64>().unwrap())
        .collect()
}

struct Race {
    time: u64,
    distance: u64,
}

fn parse_file(path: &str) -> Vec<Race> {
    let content = fs::read_to_string(path).expect("Should have been able to read the file");

    let mut lines = content.lines();
    let times = split_nums(lines.next().unwrap());
    let distances = split_nums(lines.next().unwrap());
    assert!(lines.next().is_none());

    zip(times, distances)
        .map(|(t, d)| Race {
            time: t,
            distance: d,
        })
        .collect()
}

fn main() {
    let races = parse_file("./src/bin/day6_part1/puzzle.txt");

    let answer = races
        .into_iter()
        .map(|race| {
            (1..race.time)
                .map(|i| i * (race.time - i))
                .filter(|d| d > &race.distance)
                .count()
        })
        .fold(1, |x, y| x * y);

    println!("Answer: {}", answer);
}
