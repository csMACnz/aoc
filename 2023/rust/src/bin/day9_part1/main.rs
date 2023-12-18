use std::fs;

fn parse_file(path: &str) -> Vec<Vec<i64>> {
    fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .lines()
        .map(|l| l.split(" ").map(|c| c.parse::<i64>().unwrap()).collect())
        .collect()
}

fn part_1(path: &str) -> i64 {
    let lines = parse_file(path);

    lines
        .iter()
        .map(|l| {
            let mut results = Vec::new();
            let mut active = l.clone();
            results.push(active.to_owned());
            loop {
                active = active
                    .iter()
                    .zip(active[1..].into_iter())
                    .map(|(a, b)| b - a)
                    .collect();
                if active.iter().all(|i| *i == 0) {
                    break;
                } else {
                    // println!("{:?}", active);
                    results.push(active.to_owned());
                }
            }
            let ret = results
                .iter()
                .rev()
                .fold(0_i64, |acc, l| acc + l.last().unwrap());

            // println!("{:?} + {ret}", l);

            ret
        })
        .reduce(|acc, x| acc + x)
        .unwrap()
}

fn main() {
    let answer = part_1("./src/bin/day9_part1/sample.txt");

    println!("Answer: {}", answer);
}

#[test]
fn can_parse_sample() {
    let answer = part_1("./src/bin/day9_part1/sample.txt");

    assert_eq!(answer, 114);
}

#[test]
fn can_parse_puzzle() {
    let answer = part_1("./src/bin/day9_part1/puzzle.txt");

    assert_eq!(answer, 1938731307);
}
