use std::fs;

fn main() {
    let content = fs::read_to_string("./src/bin/day1_part1/puzzle.txt")
        .expect("Should have been able to read the file");

    let mut total = 0;
    for line in content.lines() {

        let mut it = line.chars().filter_map(|c|{ c.to_digit(10) });
        let first = it.next().unwrap();
        let last = match it.last() {
            Some(l)=> l,
            None=>first
        };

        println!("{}{}", first, last);
        total += first * 10 + last;
    }
    println!("Answer: {}", total);
    
}
