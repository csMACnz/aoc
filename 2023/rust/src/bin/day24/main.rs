use std::{collections::HashSet, f64::EPSILON, fs, str::FromStr};

type Position = (i64, i64, i64);
type Velocity = (i64, i64, i64);

#[derive(Clone)]
struct Stone {
    position: Position,
    velocity: Velocity,
}

fn tuple_part(part: &(i64, i64, i64), a: usize, b: usize) -> (i64, i64) {
    match (a, b) {
        (0, 1) => (part.0, part.1),
        (1, 0) => (part.1, part.0),
        (1, 2) => (part.1, part.2),
        (2, 1) => (part.2, part.1),
        (2, 0) => (part.2, part.0),
        (0, 2) => (part.0, part.2),
        _ => panic!("not supported indexes"),
    }
}

fn tuple_i(part: &(i64, i64, i64), i: usize) -> i64 {
    match i {
        0 => part.0,
        1 => part.1,
        2 => part.2,
        _ => panic!("not supported indexes"),
    }
}

impl Stone {
    fn position_part(&self, a: usize, b: usize) -> (i64, i64) {
        tuple_part(&self.position, a, b)
    }
    fn velocity_part(&self, i: usize) -> i64 {
        tuple_i(&self.velocity, i)
    }
}

// Returns 1 if the lines intersect, otherwise 0. In addition, if the lines
// intersect the intersection point may be stored in the floats i_x and i_y.
fn two_line_intersection(
    p0_x: f64,
    p0_y: f64,
    v0_x: f64,
    v0_y: f64,
    p1_x: f64,
    p1_y: f64,
    v1_x: f64,
    v1_y: f64,
) -> Option<(f64, f64)> {
    let denom = -v1_x * v0_y + v0_x * v1_y;
    if denom.abs() < EPSILON {
        return None; // No collision
    }
    let denom_positive: bool = denom > 0.0;

    let s02_x = p0_x - p1_x;
    let s02_y = p0_y - p1_y;
    let s_numer = v0_x * s02_y - v0_y * s02_x;
    if (s_numer < 0.0) == denom_positive {
        return None; // No collision
    }

    let t_numer = v1_x * s02_y - v1_y * s02_x;
    if (t_numer < 0.0) == denom_positive {
        return None; // No collision
    }

    // if ((s_numer > denom) == denomPositive) || ((t_numer > denom) == denomPositive) {
    //     return None; // No collision
    // }

    let t0 = (v1_x * (p0_y - p1_y) - v1_y * (p0_x - p1_x)) / denom;
    let t1 = (-v0_y * (p0_x - p1_x) + v0_x * (p0_y - p1_y)) / denom;

    if t0 < EPSILON && t1 < EPSILON {
        return None; // No collision
    }
    return Some((t0, t1));
}

// https://www.geeksforgeeks.org/program-for-point-of-intersection-of-two-lines/
fn line_line_intersection(
    a: (f64, f64),
    b: (f64, f64),
    c: (f64, f64),
    d: (f64, f64),
) -> Option<(f64, f64)> {
    let x1 = a.0;
    let y1 = a.1;
    let x2 = b.0;
    let y2 = b.1;

    let x3 = c.0;
    let y3 = c.1;
    let x4 = d.0;
    let y4 = d.1;

    let d = ((x1 - x2) * (y3 - y4)) - ((y1 - y2) * (x3 - x4));

    if d.abs() < f64::EPSILON {
        // The lines are parallel.
        None
    } else {
        let x = (((x1 * y2) - (y1 * x2)) * (x3 - x4)) - ((x1 - x2) * ((x3 * y4) - (y3 * x4))) / d;
        let y = (((x1 * y2) - (y1 * x2)) * (y3 - y4)) - ((y1 - y2) * ((x3 * y4) - (y3 * x4))) / d;
        Some((x, y))
    }
    // // Line AB represented as a1x + b1y = c1
    // let a1 = b.1 - a.1;
    // let b1 = a.0 - b.0;
    // let c1 = (a1 * a.0) + (b1 * a.1);

    // // Line CD represented as a2x + b2y = c2
    // let a2 = d.1 - c.1;
    // let b2 = c.0 - d.0;
    // let c2 = (a2 * c.0) + (b2 * c.1);

    // let determinant = a1 * b2 - a2 * b1;

    // if determinant.abs() < f64::EPSILON {
    //     // The lines are parallel.
    //     None
    // } else {
    //     let x = ((b2 * c1) - (b1 * c2)) / determinant;
    //     let y = ((a1 * c2) - (a2 * c1)) / determinant;
    //     Some((x, y))
    // }
}

fn t(p: i64, v: i64, x: f64) -> f64 {
    if v == 0 {
        0.0
    } else {
        // x' = px + vx * t'
        // t' = (x' - px) / vx
        (x - (p as f64)) / (v as f64)
    }
}

fn intersect(
    first: ((i64, i64), (i64, i64)),
    second: ((i64, i64), (i64, i64)),
) -> Option<(i64, i64)> {
    if let Some((tf, ts)) = two_line_intersection(
        first.0 .0 as f64,
        first.0 .1 as f64,
        first.1 .0 as f64,
        first.1 .1 as f64,
        second.0 .0 as f64,
        second.0 .1 as f64,
        second.1 .0 as f64,
        second.1 .1 as f64,
    ) {
        // if let Some((x, y)) = line_line_intersection(
        //     (first.0 .0 as f64, first.0 .1 as f64),
        //     (
        //         (first.0 .0 + (1 * first.1 .0)) as f64,
        //         (first.0 .1 + (1 * first.1 .1)) as f64,
        //     ),
        //     (second.0 .0 as f64, second.0 .1 as f64),
        //     (
        //         (second.0 .0 + (1 * second.1 .0)) as f64,
        //         (second.0 .1 + (1 * second.1 .1)) as f64,
        //     ),
        // ) {
        // if tf >= EPSILON
        //     && ts >= EPSILON
        {
            // must be in bounds

            let xf = first.0 .0 as f64 + (tf * first.1 .0 as f64);
            let yf = first.0 .1 as f64 + (tf * first.1 .1 as f64);
            // let xs = second.0 .0 as f64 + (ts * second.1 .0 as f64);
            // let ys = second.0 .1 as f64 + (ts * second.1 .1 as f64);
            // assert!((xf-xs).abs() < 1.0); // fails
            // assert!((yf-ys).abs() < 1.0); //fails
            return Some((f64::round(xf) as i64, f64::round(yf) as i64));
        }
    }
    None
}

impl FromStr for Stone {
    type Err = String;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let (pos, vel) = s.split_once(" @ ").unwrap();
        let pos = three_split_num(pos);
        let vel = three_split_num(vel);
        Ok(Stone {
            position: pos,
            velocity: vel,
        })
    }
}

fn three_split_num(pos: &str) -> (i64, i64, i64) {
    let nums: [i64; 3] = pos
        .split(", ")
        .map(|s| s.trim().parse::<i64>().unwrap())
        .collect::<Vec<_>>()
        .try_into()
        .unwrap();
    (nums[0], nums[1], nums[2])
}

fn parse_file(path: &str) -> Vec<Stone> {
    fs::read_to_string(path)
        .expect("Should have been able to read the file")
        .lines()
        .map(|l| l.parse().unwrap())
        .collect()
}

fn part_1(path: &str, test_area_min: i64, test_area_max: i64) -> u64 {
    let stones = parse_file(path);
    let mut count = 0;
    for i in 0..stones.len() {
        for j in (i + 1)..stones.len() {
            let a = &stones[i];
            let b = &stones[j];
            if let Some((x, y)) = intersect(
                ((a.position.0, a.position.1), (a.velocity.0, a.velocity.1)),
                ((b.position.0, b.position.1), (b.velocity.0, b.velocity.1)),
            ) {
                // must be forward in time
                if x >= test_area_min
                    && x <= test_area_max
                    && y >= test_area_min
                    && y <= test_area_max
                {
                    count += 1;
                }
            }
        }
    }
    count
}

fn test(stones: &Vec<Stone>, x_index: usize, y_index: usize) -> Option<((i64, i64), (i64, i64))> {
    let min = stones.len().min(10);
    for wx in (-400)..=(400) {
        'find_w: for wy in (-400)..=(400) {
            if wy == 0 && wx == 0 {
                continue;
            }
            //p[i] + (v[i]-w)*t[i] = p[j] + (v[j]-w)*t[j]
            let w = (wx, wy);
            let mut count = 0;
            let mut verify = HashSet::new();
            for i in 0..stones.len() {
                let a = &stones[i];
                let first = (
                    a.position_part(x_index, y_index),
                    (
                        a.velocity_part(x_index) - w.0,
                        a.velocity_part(y_index) - w.1,
                    ),
                );
                for j in (i + 1)..stones.len() {
                    let b = &stones[j];
                    if let Some(r) = intersect(
                        first,
                        (
                            b.position_part(x_index, y_index),
                            (
                                b.velocity_part(x_index) - w.0,
                                b.velocity_part(y_index) - w.1,
                            ),
                        ),
                    ) {
                        verify.insert(r);
                        count += 1;
                        if count > min {
                            // println!("{verify:?}");
                            if verify.len() > 1 {
                                continue 'find_w;
                            }
                            // found at least 10 that intersect with this w - winning
                            return Some(((r.0, r.1), w));
                        }
                    }
                }
            }
        }
    }
    None
}

fn part_2(path: &str) -> u64 {
    let stones = parse_file(path);
    //(p1,v1) (p2,v2) (p3,v3) solve for (r,w)
    // collision point == r + t[i]*w == p[i] + v[i]*t[i]
    // r = p[i] + (v[i]-w)*t[i] || d = p + v*t format
    // 3 unknowns, r, w, t[i] for each equation set.
    // t[i] = (r - p[i]) / (v[i]-w) -> true for x, y, and z (for each i)
    // r = p[i] + (v[i]-w)*t[i] = p[j] + (v[j]-w)*t[j] | we can try diffferent w values and solve for t[i]/t[j] using itersection detection
    // r = p[i] + v[i]*t[i] -w*t[i]
    // r == collision point - t[i]*w

    let xy = test(&stones, 0, 1).unwrap();
    let zx = test(&stones, 2, 0).unwrap();
    let yz = test(&stones, 1, 2).unwrap();
    println!("xy({:?}),zx({:?}),yz({:?})", xy, zx, yz);
    assert_eq!(xy.0 .0, zx.0 .1);
    assert_eq!(xy.0 .1, yz.0 .0);
    assert_eq!(zx.0 .0, yz.0 .1);
    println!("start at ??? velocity ({:?},{:?},{:?})", xy, zx, yz);
    (xy.0 .0 + zx.0 .0 + yz.0 .0) as u64
}

fn main() {
    let answer1 = part_1(
        "./src/bin/day24/puzzle.txt",
        200000000000000,
        400000000000000,
    );
    let answer2 = part_2("./src/bin/day24/puzzle.txt");

    println!("Part1: {}", answer1);

    println!("Part2: {}", answer2);
}

#[test]
fn can_parse_part1_sample() {
    let answer = part_1("./src/bin/day24/sample.txt", 7, 27);

    assert_eq!(answer, 2);
}

#[test]
fn can_parse_part1_puzzle() {
    let answer = part_1(
        "./src/bin/day24/puzzle.txt",
        200000000000000,
        400000000000000,
    );

    assert_ne!(answer, 19022);
    assert_eq!(answer, 26657);
}

#[test]
fn can_parse_part2_sample() {
    let answer = part_2("./src/bin/day24/sample.txt");
    assert_eq!(answer, 47);
}

#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day24/puzzle.txt");

    assert_eq!(answer, 828418331313365);
}
