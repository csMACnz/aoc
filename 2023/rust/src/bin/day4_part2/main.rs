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

fn match_count(card: &Card) -> usize {
    card.winning_numbers.intersection(&card.your_numbers).count()
}

fn main() {
    let cards = parse_file("./src/bin/day4_part1/puzzle.txt");

    let mut scores: Vec<usize> = cards.iter().map(|_|{1}).collect();

    for (index, card) in cards.iter().enumerate() {
        let score = match_count(card);
        for n in index+1..=(index+score) {
            scores[n] = scores[n] + scores[index]
        }
    } 

    let answer: usize = scores.iter().sum();

    println!("Answer: {}", answer);
    
}
