use std::{fs, collections::HashMap};

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
    else if c == '*' {
        Symbol::Marker
    }
    else {
        Symbol::Nothing
    }
}

fn parse_file(path: &str) -> Vec<Vec<Symbol>> {
    let content = fs::read_to_string(path)
    .expect("Should have been able to read the file");
    content.lines().map(|line|{line.chars().map(to_symbol).collect()}).collect()
}

fn get_markers(grid: &Vec<Vec<Symbol>>, start_index: (usize, usize), word_width: usize, row_width: usize) -> Vec<(usize, usize)> {
    let mut result = Vec::new();
    if start_index.0 > 0 && grid[start_index.1][start_index.0-1] == Symbol::Marker {
        result.push((start_index.1, start_index.0-1));
    }
    if start_index.0 + word_width < row_width - 1 && grid[start_index.1][start_index.0+word_width] == Symbol::Marker {
        result.push((start_index.1, start_index.0+word_width));
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
        for i in safe_left_bound..=safe_right_bound {
            if grid[start_index.1-1][i] == Symbol::Marker {
                result.push((start_index.1-1,i));
            }
        }
    }
    if start_index.1 < grid.len() -1 {
        for i in safe_left_bound..=safe_right_bound {
            if grid[start_index.1+1][i] == Symbol::Marker {
                result.push((start_index.1+1,i));
            }
        }
    }
    result
}

fn main() {
    let grid: Vec<Vec<_>> = parse_file("./src/bin/day3_part1/puzzle.txt");
    let line_width = grid[0].len();
    let mut sum = 0;
    let mut map = HashMap::new();
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

                for cogPos in get_markers(&grid, (i, row), word_width, line_width) {
                    if let Some(first) = map.get_key_value(&cogPos) {
                        // println!("{} * {}", first.1, num);
                        sum += first.1 * num;
                    } else {
                        map.insert(cogPos, num);
                    }
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
