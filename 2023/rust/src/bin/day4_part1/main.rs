use std::{fs, collections::HashSet};

struct Card {
    number: u32,
    winning_numbers: HashSet<u32>,
    your_numbers: HashSet<u32>
}

fn parse_numbers(nums: &str)-> HashSet<u32> {
    nums.split(" ").filter(|x| !x.is_empty()).map(|n|{n.parse().unwrap()}).collect()
}

fn parse_line(line: &str) -> Card {
    let (left, right) = line.split_once(": ").unwrap();
    let card_number = left.split(" ").last().unwrap().parse().unwrap();
    let (winning_str, your_str) = right.split_once(" | ").unwrap();
    let winning_numbers = parse_numbers(winning_str);
    let your_numbers = parse_numbers(your_str);

    Card {
        number: card_number,
        winning_numbers: winning_numbers,
        your_numbers: your_numbers
    }
}

fn parse_file(path: &str) -> Vec<Card> {
    let content = fs::read_to_string(path)
    .expect("Should have been able to read the file");
    
    content.lines().map(parse_line).collect()
}

fn score(card: &Card) -> u32 {
    let match_count = card.winning_numbers.intersection(&card.your_numbers).count().try_into().unwrap();
    match match_count  {
        0 => 0,
        x => 2_u32.pow(x-1)
    }
}

fn main() {
    let cards = parse_file("./src/bin/day4_part1/puzzle.txt");

    let answer: u32 = cards.iter().map(score).sum();

    println!("Answer: {}", answer);
    
}
