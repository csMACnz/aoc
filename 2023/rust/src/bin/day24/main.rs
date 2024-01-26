use std::{collections::HashSet, f64::EPSILON, fs, ops::Sub, str::FromStr};

#[derive(Clone, Copy, PartialEq, Eq, Hash)]
struct Point2dI64 {
    x: i64,
    y: i64,
}

impl Point2dI64 {
    fn as_f64(&self) -> Point2dF64 {
        Point2dF64 {
            x: self.x as f64,
            y: self.y as f64,
        }
    }
}

impl Sub for Point2dI64 {
    type Output = Self;

    fn sub(self, other: Self) -> Self::Output {
        Self {
            x: self.x - other.x,
            y: self.y - other.y,
        }
    }
}

#[derive(Clone, Copy)]
struct Point2dF64 {
    x: f64,
    y: f64,
}

#[derive(Clone)]
struct Point3dI64 {
    x: i64,
    y: i64,
    z: i64,
}

impl Point3dI64 {
    fn xy(&self) -> Point2dI64 {
        Point2dI64 {
            x: self.x,
            y: self.y,
        }
    }
    fn yz(&self) -> Point2dI64 {
        Point2dI64 {
            x: self.y,
            y: self.z,
        }
    }
    fn xz(&self) -> Point2dI64 {
        Point2dI64 {
            x: self.x,
            y: self.z,
        }
    }
    fn part(&self, a: usize, b: usize) -> Point2dI64 {
        match (a, b) {
            (0, 1) => (self.xy()),
            (1, 2) => (self.yz()),
            (0, 2) => (self.xz()),
            _ => panic!("not supported indexes"),
        }
    }
    fn at(&self, i: usize) -> i64 {
        match i {
            0 => self.x,
            1 => self.y,
            2 => self.z,
            _ => panic!("not supported indexes"),
        }
    }
}

#[derive(Clone)]
struct Stone {
    position: Point3dI64,
    velocity: Point3dI64,
}

impl Stone {
    fn position_part(&self, a: usize, b: usize) -> Point2dI64 {
        self.position.part(a, b)
    }
}

// Returns 1 if the lines intersect, otherwise 0. In addition, if the lines
// intersect the intersection point may be stored in the floats i_x and i_y.
fn two_line_intersection(
    p0: Point2dF64,
    v0: Point2dF64,
    p1: Point2dF64,
    v1: Point2dF64,
) -> Option<(f64, f64)> {
    let denom = -v1.x * v0.y + v0.x * v1.y;
    if denom.abs() < EPSILON {
        return None; // No collision
    }
    let denom_positive: bool = denom > 0.0;

    let s02_x = p0.x - p1.x;
    let s02_y = p0.y - p1.y;
    let s_numer = v0.x * s02_y - v0.y * s02_x;
    if (s_numer < 0.0) == denom_positive {
        return None; // No collision
    }

    let t_numer = v1.x * s02_y - v1.y * s02_x;
    if (t_numer < 0.0) == denom_positive {
        return None; // No collision
    }

    // if ((s_numer > denom) == denomPositive) || ((t_numer > denom) == denomPositive) {
    //     return None; // No collision
    // }

    let t0 = (v1.x * (p0.y - p1.y) - v1.y * (p0.x - p1.x)) / denom;
    let t1 = (-v0.y * (p0.x - p1.x) + v0.x * (p0.y - p1.y)) / denom;

    return Some((t0, t1));
}

fn intersect(
    first: (Point2dI64, Point2dI64),
    second: (Point2dI64, Point2dI64),
) -> Option<Point2dI64> {
    let p0 = first.0.as_f64();
    let p1 = second.0.as_f64();
    let v0 = first.1.as_f64();
    if let Some((t1, t2)) = two_line_intersection(p0, v0, p1, second.1.as_f64()) {
        if t2 > EPSILON && t1 > EPSILON {
            let xf = p0.x + (t1 * v0.x);
            let yf = p0.y + (t1 * v0.y);

            return Some(Point2dI64 {
                x: f64::round(xf) as i64,
                y: f64::round(yf) as i64,
            });
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
            position: Point3dI64 {
                x: pos.0,
                y: pos.1,
                z: pos.2,
            },
            velocity: Point3dI64 {
                x: vel.0,
                y: vel.1,
                z: vel.2,
            },
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
            if let Some(p) = intersect(
                (a.position.xy(), a.velocity.xy()),
                (b.position.xy(), b.velocity.xy()),
            ) {
                // must be forward in time
                if p.x >= test_area_min
                    && p.x <= test_area_max
                    && p.y >= test_area_min
                    && p.y <= test_area_max
                {
                    count += 1;
                }
            }
        }
    }
    count
}

fn test(
    stones: &Vec<Stone>,
    x_index: usize,
    y_index: usize,
    search_range: (i64, i64),
) -> Option<(Point2dI64, Point2dI64)> {
    let min = stones.len().min(10);
    for wx in search_range.0..=search_range.1 {
        'find_w: for wy in search_range.0..=search_range.1 {
            if wy == 0 && wx == 0 {
                continue;
            }
            //p[i] + (v[i]-w)*t[i] = p[j] + (v[j]-w)*t[j]
            let w = Point2dI64 { x: wx, y: wy };
            let mut count = 0;
            let mut verify = HashSet::new();
            for i in 0..stones.len() {
                let a = &stones[i];
                let first = (
                    a.position_part(x_index, y_index),
                    a.velocity.part(x_index, y_index) - w,
                );
                for j in (i + 1)..stones.len() {
                    let b = &stones[j];
                    if let Some(r) = intersect(
                        first,
                        (
                            b.position_part(x_index, y_index),
                            b.velocity.part(x_index, y_index) - w,
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
                            return Some((r, w));
                        }
                    }
                }
            }
        }
    }
    None
}

fn part_2(path: &str, search_magnitude: i64) -> u64 {
    let stones = parse_file(path);
    //(p1,v1) (p2,v2) (p3,v3) solve for (r,w)
    // collision point == r + t[i]*w == p[i] + v[i]*t[i]
    // r = p[i] + (v[i]-w)*t[i] || d = p + v*t format
    // 3 unknowns, r, w, t[i] for each equation set -> r is the collision point, is also the starting p for the stone.
    // r = p[i] + (v[i]-w)*t[i] = p[j] + (v[j]-w)*t[j]
    // p[i] + (v[i]-w)*t[i] = p[j] + (v[j]-w)*t[j] intersect at r, if at all. | we can try diffferent w values and solve for t[i]/t[j] using itersection detection

    let search_range = (-search_magnitude, search_magnitude);
    let xy = test(&stones, 0, 1, search_range).unwrap();
    let xz = test(&stones, 0, 2, search_range).unwrap();
    let yz = test(&stones, 1, 2, search_range).unwrap();

    assert_eq!(xy.0 .x, xz.0 .x);
    assert_eq!(xy.0 .y, yz.0 .x);
    assert_eq!(xz.0 .y, yz.0 .y);

    (xy.0 .x + xy.0 .y + yz.0 .y) as u64
}

fn main() {
    let answer1 = part_1(
        "./src/bin/day24/puzzle.txt",
        200000000000000,
        400000000000000,
    );
    let answer2 = part_2("./src/bin/day24/puzzle.txt", 350);

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
    let answer = part_2("./src/bin/day24/sample.txt", 10);
    assert_eq!(answer, 47);
}

#[test]
fn can_parse_part2_puzzle() {
    let answer = part_2("./src/bin/day24/puzzle.txt", 350);

    assert_eq!(answer, 828418331313365);
}
