use std::fs;

fn main() {
    let content = fs::read_to_string("./src/bin/day2_part2/puzzle.txt")
        .expect("Should have been able to read the file");
    let mut powerSum = 0;
    for line in content.lines() {
        let mut gameLine = line.split(":");
        let gameId: i32 = gameLine.next().unwrap().chars().skip(5).collect::<String>().parse().unwrap();
        let games = gameLine.last().unwrap().split(";");
        let mut red = 0;
        let mut green = 0;
        let mut blue = 0;
        for game in games {
            let colors = game.split(",");
            for colorData in colors {
                let mut frame = colorData.split(" ");
                frame.next();
                let count: i32 = frame.next().unwrap().trim().parse().unwrap();
                let color = frame.last().unwrap();
                match color {
                    "red" => red = i32::max(red, count),
                    "green" => green =i32::max(green, count),
                    "blue" => blue =i32::max(blue, count),
                    &_ => todo!()
                };
            }
        }
        
        powerSum += red * green * blue;
        println!("{}: r{} g{} b{} == {}", gameId, red, green, blue, powerSum);
    }
    
    println!("Answer: {}", powerSum);
    
}
