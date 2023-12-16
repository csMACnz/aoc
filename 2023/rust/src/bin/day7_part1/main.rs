use std::{collections::HashMap, fs};

#[derive(Debug)]
struct Hand {
    cards: [char; 5],
    bid: u64,
}

fn card_map(c: char) -> char {
    match c {
        'A' => 'E',
        'K' => 'D',
        'Q' => 'C',
        'J' => 'B',
        'T' => 'A',
        x => x,
    }
}

fn parse_hand(line: &str) -> Hand {
    let (hand, bid) = line.split_once(" ").unwrap();
    Hand {
        cards: hand
            .chars()
            .collect::<Vec<char>>()
            .try_into()
            .unwrap(),
        bid: bid.parse().unwrap(),
    }
}

fn parse_file(path: &str) -> Vec<Hand> {
    fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .lines()
        .map(parse_hand)
        .collect()
}

fn score(hand: &Hand) -> u64 {
    let mut groups: HashMap<char, u64> = Default::default();
    for c in hand.cards {
        if !groups.contains_key(&c) {
            groups.insert(c, 1);
        } else {
            groups.insert(c, groups[&c] + 1);
        }
    }
    match groups.len() {
        1 => 7,
        2 => {
            if groups.into_iter().any(|(_, v)| v == 4) {
                6
            } else {
                5
            }
        }
        3 => {
            if groups.into_iter().any(|(_, v)| v == 3) {
                4
            } else {
                3
            }
        }
        4 => 2,
        _ => 1,
    }
}

fn main() {
    let mut hands = parse_file("./src/bin/day7_part1/puzzle.txt");

    hands.sort_by(|a, b| {
        let a_score = score(&a);
        let b_score = score(&b);
        if a_score == b_score {
            a.cards.map(card_map).cmp(&b.cards.map(card_map))
        } else {
            a_score.cmp(&b_score)
        }
    });

    hands.iter().enumerate().for_each(|x| {
        println!("{:?} {}", x, score(&x.1));
    });

    let answer = hands
        .iter()
        .enumerate()
        .fold(0_u64, |acc, x| acc + (x.1.bid * ((x.0 as u64) + 1)));

    println!("Answer: {}", answer);
}
