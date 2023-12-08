use std::fs;

fn findFirst(line: &str, text: &str, digit: char, value: i32) -> Option<(usize, i32)> {
    let findResult = (line.find(text), line.find(digit));
    match findResult {
        (None, None) => None,
        (None, Some(d)) => Some((d, value)),
        (Some(t), None) => Some((t, value)),
        (Some(a), Some(b)) => Some((usize::min(a, b), value)),
    }
}
fn findLast(line: &str, text: &str, digit: char, value: i32) -> Option<(usize, i32)> {
    let findResult = (line.rfind(text), line.rfind(digit));
    match findResult {
        (None, None) => None,
        (None, Some(d)) => Some((d, value)),
        (Some(t), None) => Some((t, value)),
        (Some(a), Some(b)) => Some((usize::max(a, b), value)),
    }
}

fn main() {
    let content = fs::read_to_string("./src/bin/day1_part2/puzzle.txt")
        .expect("Should have been able to read the file");

    let values = [("one", '1', 1),("two", '2', 2),("three", '3', 3),("four", '4', 4),("five", '5', 5),("six", '6', 6),("seven", '7', 7),("eight", '8', 8),("nine", '9', 9)];

    let mut total = 0;
    for line in content.lines() {
        let first = values
            .iter()
            .filter_map(|v|{findFirst(line, v.0, v.1, v.2)})
            .min_by(|lhs, rhs|{lhs.0.cmp(&rhs.0)})
            .unwrap().1;

        let last = values
            .iter()
            .filter_map(|v|{findLast(line, v.0, v.1, v.2)})
            .max_by(|lhs, rhs|{lhs.0.cmp(&rhs.0)})
            .unwrap().1;

        println!("{}{}", first, last);
        total += first * 10 + last;
    }
    println!("Answer: {}", total);
    
}
