use std::fs;

fn main() {
    let content = fs::read_to_string("./src/bin/day1_part1/puzzle.txt")
        .expect("Should have been able to read the file");

    let mut total = 0;
    for line in content.lines() {
        let mut first = 0;
        for c in line.chars() {
            if c >= '0' && c <= '9' {
                first = c.to_digit(10).unwrap();
                break;
            }
        }
        
        let mut last = 0;
        for c in line.chars() {
            if c >= '0' && c <= '9' {
                last = c.to_digit(10).unwrap();
                
            }
        }

        println!("{}{}", first, last);
        total += first * 10 + last;
    }
    println!("Answer: {}", total);
    
}
