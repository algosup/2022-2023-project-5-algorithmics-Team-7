# Stop calculating if all the wines have less than 5% of difference with the recipe
PERCENTAGE_LIMIT = 0.05

RUN_TESTS = True


# Note: Is valid JSON
UNIT_TESTS = [
    {
        "name": "No tranfert",
        "recipe": [1],
        "volumes": [10],
        "formula": [1],
        "steps": ""
    },
    {
        "name": "Combine",
        "recipe": [.5, .5],
        "volumes": [10, 10, 20],
        "formula": [.5, .5],
        "steps": "1 -> 3\n2 -> 3"
    },
    {
        "name": "Proportions",
        "recipe": [.333, .667],
        "volumes": [10, 20, 30],
        "formula": [.333, .667],
        "steps": "1 -> 3\n2 -> 3"
    },
    {
        "name": "Crossed",
        "recipe": [.5, .5],
        "volumes": [10, 10, 5, 15],
        "formula": [.5, .5],
        "steps": "1 -> 3\n1 -> 4\n2 -> 3\n2 -> 4"
    },
    # {
    #     "name": "Split & multi-step",
    #     "recipe": [.5, .5],
    #     "volumes": [20, 10, 10, 10],
    #     "formula": [.5, .5],
    #     "steps": "1 -> 3\n1 ->4\n\n2 -> 1\n3 -> 1"
    # },
    {
        "name": "Closest",
        "recipe": [.5, .5],
        "volumes": [10, 30, 40, 20, 10],
        "formula": [.333, .667],
        "steps": "2 -> 4\n2 -> 5\n\n1 -> 2\n4 -> 2"
    },
    {
        "name": "Functional specs example",
        "recipe": [20, 40, 40],
        "volumes": [10, 20, 30, 15, 15, 35],
        "formula": [0.19, 0.381, 0.429],
        "steps": "3 -> 4\n3 -> 5\n\n1 -> 3\n2 -> 3\n\n3 -> 1\n3 -> 6\n5 -> 6"
    },
    {
        "name": "Void edge case",
        "recipe": [],
        "volumes": [],
        "formula": [],
        "steps": ""
    }
]
    # {
    #     "name": "",
    #     "recipe": [],
    #     "volumes": [],
    #     "formula": [],
    #     "steps": ""
    # },


class Node:
    def __init__(
            self,
            contents: list[list[float]],
            connections: list[tuple[int, int]],
            parent: "Node | None" = None,
            step: int = 0
            ) -> None:
        self.contents = contents
        self.connections = connections
        self.parent = parent
        self.step = step

    def score(self, target: list[float]) -> float:
        return round(min(
            score(tank, target)
            for tank in self.contents
            if any(tank)
        ), 5)

    def best_formula(self, target: list[float]) -> list[float]:
        if not target:
            return []
        return min(
            (tank for tank in self.contents if any(tank)),
            key=lambda tank: difference(tank, target)
        )

    def transfer(self, connections: list[tuple[int, int]], volumes: list[int]) -> "Node | None":
        # Calculate the tranfer speed from/into each tank
        remaining_connections = connections.copy()
        contents = [tank.copy() for tank in self.contents]
        in_tanks = [connection[0] for connection in connections]
        out_tanks = [connection[1] for connection in connections]
        extract_speed = {tank: in_tanks.count(tank) for tank in set(in_tanks)}
        insert_speed = {tank: out_tanks.count(tank) for tank in set(out_tanks)}

        # Dividing the transfert in smaller steps
        while any(insert_speed.values()):  # Still pumping, keep going
            # Calculate how much we will transfert at this step
            volumes_extract = [
                sum(contents[tank]) / speed
                for tank, speed in extract_speed.items()
            ]
            volumes_insert = [
                (volumes[tank] - sum(contents[tank])) / speed
                for tank, speed in insert_speed.items()
            ]
            volume = min(*volumes_extract, *volumes_insert)

            # Change the content in each tank accordingly
            for a, b in remaining_connections:
                factor = volume / sum(contents[a])
                for i in range(len(contents[a])):
                    t = contents[a][i] * factor
                    contents[a][i] -= t
                    contents[b][i] += t

            # Remove unnecessary connections and change speed
            for connection in remaining_connections.copy():
                a, b = connection
                if any(contents[a]) and round(sum(contents[b]), 6) < volumes[b]:
                    continue
                remaining_connections.remove(connection)
                extract_speed[a] -= 1
                insert_speed[b] -= 1
                if extract_speed[a] == 0:
                    in_tanks = list(filter(lambda j: j != a, in_tanks))
                    extract_speed.pop(a)
                if insert_speed[b] == 0:
                    out_tanks = list(filter(lambda j: j != a, out_tanks))
                    insert_speed.pop(b)

        for tank, volume in zip(contents, volumes):
            if sum(tank) != 0 and sum(tank) != volume:
                return None

        return Node(contents, connections, self, self.step+1)

    def children(self, volumes: list[int]) -> "list[Node]":
        # Split the tanks into possible inputs and outputs
        possible_in: list[int] = []
        possible_out: list[int] = []
        for i, tank in enumerate(self.contents):
            if any(tank):
                possible_in.append(i)
            else:
                possible_out.append(i)

        # Validate connections
        combinations: list[list[tuple[int, int]]] = []
        for connections in pairs_combinations_generator(possible_in, possible_out, []):
            if not connections:
                continue  # Need at least one connection
            xs = set(pair[0] for pair in connections)
            ys = set(pair[1] for pair in connections)
            if sum(volumes[i] for i in xs) != sum(volumes[i] for i in ys):
                continue  # Must be the same volumes
            combinations.append(connections)

        # Generate all the children from the possible connections
        children: list[Node] = []
        for connections in combinations:
            child = self.transfer(connections, volumes)
            if child is not None:
                children.append(child)
        return children

    def __eq__(self, other: object) -> bool:
        if isinstance(other, Node):
            return self.contents == other.contents
        return False

    def __hash__(self) -> int:
        return hash(tuple(tuple(tank) for tank in self.contents))

    def __repr__(self) -> str:
        return str(self.contents)


def load(recipe_path: str, volumes_path: str) -> tuple[list[float], list[int]]:
    # Read the recipe and volumes from files
    with open(recipe_path) as f:
        recipe = [float(line) for line in f.readlines() if line]
    with open(volumes_path) as f:
        volumes = [int(line) for line in f.readlines() if line]

    # Optionally, check for the recipe to amount to 100% (will be normalized anyway)
    # if sum(recipe) != 1:
    #     raise ValueError(f"The recipe must be a total of 100%, not {100*sum(recipe)}%")

    return recipe, volumes


def normalize(L: list[float], factor: float = 1, rounding: int | None = None) -> list[float]:
    s = sum(L)
    if not s:
        return L
    if rounding is None:
        return [x * factor / s for x in L]
    return [round(x * factor / s, rounding) for x in L]


def score(L1: list[float], L2: list[float]) -> float:
    # Squared Euclidian distance
    if not L1 or not L2:
        return 0
    return sum(
        (v1 - v2) * (v1 - v2)
        for v1, v2 in zip(normalize(L1), normalize(L2))
    )


def difference(L1: list[float], L2: list[float]) -> float:
    # Maximum Absolute Difference
    if not L1 or not L2:
        return 0
    return max(
        abs(v1 - v2)
        for v1, v2 in zip(normalize(L1), normalize(L2))
    )


def pairs_combinations_generator(L1: list[int], L2: list[int], stack: list[tuple[int, int]]):
    # Generator for all the possible combinations
    # TODO: Change order of yielding [[(1, 1), (1, 2), (1, 3), ...], ...] -> [[(1, 1)], [(1, 2)], [(1, 3)], ...]
    yield stack
    if len(stack) == len(L1) * len(L2):
        return
    # m is the last pair checked, used for ordering and thus removing duplicates
    m = max(stack, default=(-float("inf"), 0))
    for a in L1:
        for b in L2:
            pair = (a, b)
            # Thanks to https://docs.python.org/3/library/stdtypes.html#common-sequence-operations:~:text=tuples%20and%20,elements
            if pair <= m:
                continue  # Case already checked
            # TODO: Flatten from recursive to iterative to prevent RecursionError
            yield from pairs_combinations_generator(L1, L2, stack+[pair])


def solve(recipe: list[float], volumes: list[int], depth_limit: int = 10, percentage_limit: float = PERCENTAGE_LIMIT) -> Node:
    # Initialization
    recipe = normalize(recipe)
    start = Node([
        [volumes[i] if i == j else 0 for j in range(len(recipe))]
        for i in range(len(volumes))
    ], [])
    if difference(start.best_formula(recipe), recipe) < percentage_limit:
        return start
    best_node = start
    best_score = start.score(recipe)
    children: set[Node] = {start}
    done: set[Node] = set()

    # We limit the depth at which we check
    for depth in range(depth_limit):
        if not children:
            break  # We checked every possible combination
        parents, children = children, set()
        done.update(parents)

        # Generate all the children
        for parent in parents:
            for child in parent.children(volumes):
                if child in done:
                    continue
                children.add(child)
                # Memorize the best one yet in case we exceed the depth limit
                score = child.score(recipe)
                if score < best_score:
                    best_score, best_node = score, child

        # Early stopping
        for child in sorted(children, key=lambda child: child.score(recipe)):
            if difference(child.best_formula(recipe), recipe) < percentage_limit:
                return child

    return best_node


def bruteforce(recipe: list[float], volumes: list[int], depth_limit: int = 8) -> Node:
    # Initialization
    recipe = normalize(recipe)
    start = Node([
        [volumes[i] if i == j else 0 for j in range(len(recipe))]
        for i in range(len(volumes))
    ], [])
    best_node = start
    best_score = start.score(recipe)
    todo: list[Node] = [start]
    done: list[Node] = []
    last_step = 0

    try:

        # We limit the depth at which we check
        while todo and todo[0].step <= depth_limit:
            parent = todo.pop(0)
            done.append(parent)
            if parent.step > last_step:
                last_step = parent.step
                print(last_step, len(todo), len(done))

            # Generate all the children
            for child in parent.children(volumes):
                todo.append(child)
                if child.score(recipe) < best_score:
                    best_score = child.score(recipe)
                    best_node = child
    
    except KeyboardInterrupt:
        pass # Manually stop the generation

    return best_node


def generate_steps(node: Node | None) -> str:
    # Backtrack the path and format
    stack: list[Node] = []
    while node is not None and node.parent is not None:
        stack.append(node)
        node = node.parent
    
    lines: list[str] = []
    for node in reversed(stack):
        for a, b in sorted(node.connections):
            lines.append(f"{a + 1} -> {b + 1}")
        lines.append("")
    
    return "\n".join(lines[:-1])


def save(node: Node, formula: list[float], formula_path: str, steps_path: str) -> None:
    percentages = "\n".join(str(x) for x in normalize(formula, factor=100, rounding=3))
    steps = generate_steps(node)

    with open(formula_path, "w") as f:
        f.write(percentages)
    with open(steps_path, "w") as f:
        f.write(steps)


def run_unit_tests(tests: list[dict[str, str | list[float]]]) -> None:
    for i, test in enumerate(tests):
        # Load test data
        try:
            recipe, volumes, expected_formula, expected_steps = test["recipe"], test["volumes"], test["formula"], test["steps"]
        except KeyError as e:
            raise e from ValueError(f"Unit test {i + 1} build incorrectly")

        # Check test
        try:
            solution = solve(recipe, volumes)

            expected_formula = normalize(expected_formula, rounding=3)
            actual_formula = normalize(solution.best_formula(recipe), rounding=3)
            assert actual_formula == expected_formula, f"Test {i + 1} failed: Expected {expected_formula}, got {actual_formula}"

            actual_steps = generate_steps(solution)
            assert actual_steps == expected_steps, f"Test {i + 1} failed: Expected {expected_steps!r}, got {actual_steps!r}"
        except AssertionError:
            raise
        except Exception as e:
            raise e from RuntimeError(f"Test {i + 1} failed")


def main():
    # TODO: argparse
    # import argparse
    # parser = argparse.ArgumentParser(
    #     prog="Wine Mixer",
    #     description="Wine mixing steps generator",
    # )
    # parser.add_argument("recipe")
    # parser.add_argument("volumes")
    # parser.add_argument("") # TODO: Default to stdout
    # parser.add_argument("") # TODO: Default to stdout

    recipe, volumes = load("test/recipe.txt", "test/volumes.txt")
    if not recipe:
        save(None, [], "test/formula.txt", "test/steps.txt")
        return
    if len(recipe) > len(volumes):
        raise ValueError(f"Cannot store {len(recipe)} wines in only {len(volumes)} tanks")



    # contents = [[volumes[i] if i == j else 0 for j in range(len(recipe))] for i in range(len(volumes))]
    # print(contents)
    # print(recipe)
    # print([score(tank, recipe) for tank in contents if any(tank)])
    # print("===", Node([
    #     [volumes[i] if i == j else 0 for j in range(len(recipe))]
    #     for i in range(len(volumes))
    # ], []).score(recipe))
    # return



    print("Expected:", recipe)
    print("Tank sizes:", volumes)

    print()
    solution = solve(recipe, volumes)
    formula = solution.best_formula(recipe)
    index = solution.contents.index(formula)
    step, node = -1, solution
    while node is not None:
        step, node = step + 1, node.parent
    output_formula = normalize(formula, factor=sum(recipe), rounding=3)
    output_volumes = [round(x, 3) for x in formula]
    steps = generate_steps(solution)

    print("Best formula found in tank", index + 1, "after", step, "steps.")
    print("The formula is:", output_formula)
    print("With the following volumes:", output_volumes)

    print()
    print("Steps:")
    print(steps)

    save(solution, formula, "test/formula.txt", "test/steps.txt")



    print()
    from time import time
    start_time = time()
    bf_solution = bruteforce(recipe, volumes)
    end_time = time()
    print("Took:", end_time - start_time)
    bf_formula = bf_solution.best_formula(recipe)
    bf_index = bf_solution.contents.index(bf_formula)
    bf_step, bf_node = -1, bf_solution
    while bf_node is not None:
        print(bf_node.contents)
        bf_step, bf_node = bf_step + 1, bf_node.parent
    bf_output_formula = normalize(bf_formula, factor=sum(recipe), rounding=3)
    bf_output_volumes = [round(x, 3) for x in bf_formula]
    bf_steps = generate_steps(bf_solution)

    print("Best formula found in tank", bf_index + 1, "after", bf_step, "steps.")
    print("The formula is:", bf_output_formula)
    print("With the following volumes:", bf_output_volumes)

    print()
    print("Steps:")
    print(bf_steps)


if RUN_TESTS:
    run_unit_tests(UNIT_TESTS)

if __name__ == "__main__":
    main()
