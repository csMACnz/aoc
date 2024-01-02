use std::fs;

struct Pond {
    tiles: Vec<Vec<char>>,
    tiles_pivot: Vec<Vec<char>>,
}

fn parse_file(path: &str) -> Vec<Pond> {
    fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .split("\r\n\r\n")
        .map(|p| {
            let tiles: Vec<Vec<char>> = p.lines().map(|l| l.chars().collect()).collect();
            let tiles_pivot = pivot(&tiles);
            Pond { tiles, tiles_pivot }
        })
        .collect()
}

fn pivot(tiles: &Vec<Vec<char>>) -> Vec<Vec<char>> {
    (0..tiles[0].len())
        .map(|i| tiles.iter().map(|inner| inner[i]).collect::<Vec<char>>())
        .collect()
}

fn find_mirror_index(tiles: &[Vec<char>]) -> Option<usize> {
    for (mirror_index, items) in tiles.windows(2).enumerate() {
        if items[0] == items[1] {
            let mut found = true;
            for i in (mirror_index + 1)..tiles.len() {
                let left_index = mirror_index + 1 - (i - mirror_index);
                if tiles[i] != tiles[left_index] {
                    found = false;
                    break;
                }
                if left_index == 0 {
                    break;
                }
            }
            if found {
                return Some(mirror_index);
            }
        }
    }
    None
}

fn find_mirror_index_2(tiles: &[Vec<char>]) -> Option<usize> {
    for (mirror_index, items) in tiles.windows(2).enumerate() {
        if let Some(mut first_different_count) = count_differences(&items[0], &items[1]) {
            let mut found = true;
            if mirror_index > 0 {
                for i in (mirror_index + 2)..tiles.len() {
                    let left_index = mirror_index + 1 - (i - mirror_index);
                    match (
                        first_different_count,
                        count_differences(&tiles[i], &tiles[left_index]),
                    ) {
                        (_, None) | (1, Some(1)) => {
                            found = false;
                            break;
                        }
                        (0, Some(1)) => {
                            first_different_count += 1;
                        }
                        (1, Some(0)) | (0, Some(0)) => {}
                        _ => unreachable!(),
                    };
                    if left_index == 0 {
                        break;
                    }
                }
            }
            // println!("{}: {}-{}", mirror_index, found, first_different_count);
            if found && first_different_count == 1 {
                return Some(mirror_index);
            }
        }
    }
    None
}

fn count_differences(first: &[char], second: &[char]) -> Option<u64> {
    for x in 0..first.len() {
        if first[x] != second[x] {
            for y in (x + 1)..first.len() {
                if first[y] != second[y] {
                    return None;
                }
            }
            return Some(1);
        };
    }
    Some(0)
}

fn score_part1(pond: &Pond) -> u64 {
    // find the mirror point and score
    if let Some(value) = find_mirror_index(&pond.tiles) {
        return 100 * (value as u64 + 1);
    }

    if let Some(value) = find_mirror_index(&pond.tiles_pivot) {
        return value as u64 + 1;
    }

    unreachable!()
}

fn score_part2(pond: &Pond) -> u64 {
    // println!("Pond:");
    // find the mirror point and score
    if let Some(value) = find_mirror_index_2(&pond.tiles) {
        return 100 * (value as u64 + 1);
    }

    if let Some(value) = find_mirror_index_2(&pond.tiles_pivot) {
        return value as u64 + 1;
    }

    unreachable!()
}

fn part_1(path: &str) -> u64 {
    let ponds = parse_file(path);
    ponds.iter().map(score_part1).sum()
}

fn part_2(path: &str) -> u64 {
    let ponds = parse_file(path);
    ponds.iter().map(score_part2).sum()
}

fn main() {
    let answer1 = part_1("./src/bin/day13/puzzle.txt");
    let answer2 = part_2("./src/bin/day13/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample() {
    let answer = part_1("./src/bin/day13/part1_sample.txt");

    assert_eq!(answer, 405);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1("./src/bin/day13/puzzle.txt");

    assert_eq!(answer, 35521);
}

#[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day13/part1_sample.txt");

    assert_eq!(answer, 400);
}

#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day13/puzzle.txt");

    assert_eq!(answer, 34795);
}
