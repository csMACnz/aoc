use std::{fs};

struct Garden {
    seeds: Vec<u64>,
    transforms: Vec<Map>
}

struct Map {
    name: String,
    lines: Vec<(u64, u64, u64)>
}


fn parse_nums(line: &str) -> Vec<u64> {
    line.split(" ").map(|x| x.parse::<u64>().unwrap()).collect()
}

fn parse_file(path: &str) -> Garden {
    let content = fs::read_to_string(path)
    .expect("Should have been able to read the file");
    
    let mut lines = content.lines();
    let seeds = parse_nums(lines.next().unwrap().split_once(": ").unwrap().1);
    lines.next();
    let mut transforms = vec!();
    let mut transform: Vec<_> = vec!();
    let mut name: &str = "";
    for line in lines {
        if line.is_empty() {
            transforms.push(Map{name: name.into(), lines: transform.clone()});
        }
        else if line.contains("map:") {
            name = line.split_once(" ").unwrap().0.split_once("-to-").unwrap().1;
            transform.clear();
        }
        else {
            let items = parse_nums(line);
            transform.push((items[0], items[1], items[2]));
        }
    }
    transforms.push(Map{name: name.into(), lines: transform.clone()});
    Garden {
        seeds: seeds,
        transforms: transforms
    }
}


fn gen_seeds(seeds: Vec<u64>) -> Vec<u64> {
    seeds.chunks(2).map(|x|x[0]..(x[0]+x[1])).flatten().collect()
}

fn main() {
    let garden = parse_file("./src/bin/day5_part2/puzzle.txt");

    let mut min = u64::MAX;
    for seed in gen_seeds(garden.seeds) {
        let mut value = seed;
        for transform_set in garden.transforms.iter() {
            // let mut previous = value;
            for transform in transform_set.lines.iter() {
                let range_start = transform.1;
                let new_range_start = transform.0;
                let range_length = transform.2;
                // println!("{range_start} {new_range_start} {range_length}");
                let range_end = range_start + range_length - 1;
                if value >= range_start && value <= range_end {
                    value = new_range_start + (value - range_start);
                    break;
                }
            }
            // println!("{} {previous} -> {value}", transform_set.name);
        }
        // println!("{value}");
        if value < min {
            min = value;
        }
    }

    let answer = min;

    println!("Answer: {}", answer);
    
}
