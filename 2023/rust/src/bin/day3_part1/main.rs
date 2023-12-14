use std::{fs, vec, cmp::{max, min}};

#[derive(PartialEq)]
enum Symbol {
    Nothing,
    Marker,
    Digit(u32)
}

fn to_symbol(c: char) -> Symbol {
    if let Some(d) = c.to_digit(10) {
        Symbol::Digit(d)
    }
    else if c == '.' {
        Symbol::Nothing
    }
    else {
        Symbol::Marker
    }
}

fn has_marker(grid: &Vec<Vec<Symbol>>, start_index: (usize, usize), word_width: usize, row_width: usize) -> bool {
    if start_index.0 > 0 && grid[start_index.1][start_index.0-1] == Symbol::Marker {
        return true;
    }
    if start_index.0 + word_width < row_width - 1 && grid[start_index.1][start_index.0+word_width] == Symbol::Marker {
        return true;
    }

    let safe_left_bound = if start_index.0 > 0 {
        start_index.0 - 1
    }
    else {
        start_index.0
    };
    
    let safe_right_bound = if start_index.0 + word_width < row_width {
        start_index.0 + word_width
    }
    else {
        start_index.0 + word_width - 1
    };

    if start_index.1 > 0 {
        let sub = &grid[start_index.1-1][safe_left_bound..=safe_right_bound];
        if sub.contains(&Symbol::Marker) {
            return true;
        }
    }
    if start_index.1 < grid.len() -1 {
        let sub = &grid[start_index.1+1][safe_left_bound..=safe_right_bound];
        if sub.contains(&Symbol::Marker) {
            return true;
        }
    }
    false
}

fn main() {
    let content = fs::read_to_string("./src/bin/day3_part1/puzzle.txt")
        .expect("Should have been able to read the file");
    let grid: Vec<Vec<_>> = content.lines().map(|line|{line.chars().map(to_symbol).collect()}).collect();
    let line_width = grid[0].len();
    let mut sum = 0;
    for row in 0..grid.len() {
        let mut col = 0;
        loop { 
            let mut word_width = 0;
            let mut num = 0;
            let mut index = None;
            while let Symbol::Digit(d) = &grid[row][col] {
                if index == None {
                    index = Some(col);
                }
                num = num*10 + d;
                word_width = word_width+1;
                col += 1;
                if col >= line_width {
                    break;
                }
            }
            if let Some(i) = index {
                if has_marker(&grid, (i, row), word_width, line_width) {
                    sum += num;
                }
            }
            else {
                col += 1;
            }
            if col >= line_width {
                break;
            }
        }
    }


    println!("Answer: {}", sum);
    
}
