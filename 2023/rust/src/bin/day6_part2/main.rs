use std::fs;
fn parse_num(line: &str) -> u64 {
    line.split_once(":")
        .unwrap()
        .1
        .replace(" ", "")
        .parse::<u64>()
        .unwrap()
}

struct Race {
    time: u64,
    distance: u64,
}

fn parse_file(path: &str) -> Vec<Race> {
    let content = fs::read_to_string(path).expect("Should have been able to read the file");

    let mut lines = content.lines();
    let time = parse_num(lines.next().unwrap());
    let distance = parse_num(lines.next().unwrap());
    assert!(lines.next().is_none());

    vec![Race {
        time: time,
        distance: distance
    }]
}

fn main() {
    let races = parse_file("./src/bin/day6_part2/puzzle.txt");

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
