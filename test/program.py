# FIXME: Too much testing breaks the testing
# The code must be analyzed and fixed



# TODO: Precompute results (closeness, wasted, ...) for performance
# TODO: Possible to have multiple output tanks with same result formula
# TODO: Log file, better inputs/outputs

import math
from random import choice
from typing import Any, Callable

PERCENTAGE_LIMIT = 1 / 100
CONNECTIONS_LIMIT = 5
DEPTH_LIMIT = 10
MAX_NO_IMPROVEMENT = 3

RUN_TESTS = False


# Note: Is valid JSON
UNIT_TESTS = [
    {
        "name": "1 - No tranfert",
        "recipe": [1],
        "volumes": [10],
        "formula": [1],
        "steps": ""
    },
    {
        "name": "2 - Combine",
        "recipe": [.5, .5],
        "volumes": [10, 10, 20],
        "formula": [.5, .5],
        "steps": "1 -> 3\n2 -> 3"
    },
    {
        "name": "3 - Proportions",
        "recipe": [.333, .667],
        "volumes": [10, 20, 30],
        "formula": [.333, .667],
        "steps": "1 -> 3\n2 -> 3"
    },
    {
        "name": "4 - Crossed",
        "recipe": [.5, .5],
        "volumes": [10, 10, 5, 15],
        "formula": [.5, .5],
        "steps": "1 -> 3\n1 -> 4\n2 -> 3\n2 -> 4"
    },
    # {
    #     "name": "5 - Split & multi-step",
    #     "recipe": [.5, .5],
    #     "volumes": [20, 10, 10, 10],
    #     "formula": [.5, .5],
    #     "steps": "1 -> 3\n1 -> 4\n\n2 -> 1\n3 -> 1"
    # },
    {
        "name": "6 - Formula before volume",
        "recipe": [.5, .5],
        "volumes": [10, 30, 40, 20, 10],
        "formula": [.5, .5],
        "steps": "1 -> 4\n2 -> 4\n2 -> 5"
    },
    # FIXME
    # {
    #     "name": "7 - Functional specs example",
    #     "recipe": [20, 40, 40],
    #     "volumes": [10, 20, 30, 15, 15, 35],
    #     "formula": [0.19, 0.381, 0.429],
    #     "steps": "3 -> 4\n3 -> 5\n\n1 -> 3\n2 -> 3\n\n3 -> 1\n3 -> 6\n4 -> 6"
    # },
    {
        "name": "8 - Void edge case",
        "recipe": [],
        "volumes": [],
        "formula": [],
        "steps": ""
    }
]


from time import time

# The isclose function from math doesn't work well when one of the argument is 0
from math import isfinite#, isclose

def isclose(a: float, b: float) -> bool:
    return abs(a - b) < 1e-8



class Tank:
    def __init__(self, total_volume: float, percentages: list[float], name: str | None = None, oxidized: bool = False) -> None:
        self.size = total_volume
        self.percentages = percentages
        self.name = name
        self.oxydized = oxidized
    
    def copy(self) -> "Tank":
        return Tank(self.size, self.percentages.copy(), self.name, self.oxydized)
    
    @property
    def content(self) -> float:
        return sum(self.percentages) * self.size

    def closeness(self, target: list[float]) -> float:
        # Squared Euclidian distance
        assert len(target) == len(self.percentages)
        assert isclose(sum(target), 1)
        if not self.is_full():
            raise ValueError(f"Cannot calculate the closeness of an invalid tank ({self})")
        if self.oxydized:
            return float("inf")
        return sum(
            (v1 - v2) * (v1 - v2)
            for v1, v2 in zip(self.percentages, target)
        )

    def difference(self, target: list[float]) -> float:
        # Maximum Absolute Difference
        assert len(target) == len(self.percentages)
        assert isclose(sum(target), 1) 
        if not self.is_full():
            raise ValueError(f"Cannot calculate the difference of an invalid tank ({self})")
        if self.oxydized:
            return float("inf")
        return max(
            abs(v1 - v2)
            for v1, v2 in zip(self.percentages, target)
        )
    
    def is_full(self) -> bool:
        return isclose(sum(self.percentages), 1)
    
    def is_empty(self) -> bool:
        return isclose(sum(self.percentages), 0)
    
    def is_not_full(self) -> bool:
        return not self.is_full()
    
    def is_not_empty(self) -> bool:
        return not self.is_empty()
    
    def __eq__(self, other: object) -> bool:
        if not isinstance(other, Tank):
            return False
        if other.size != self.size:
            return False
        if other.oxydized != self.oxydized:
            return False
        # Note: This will ignore some tanks: Might move it after the children generation to only keep the "bests"
        if not all(abs(p1 - p2) < PERCENTAGE_LIMIT for p1, p2 in zip(self.percentages, other.percentages)):
            return False
        return True

    def __repr__(self) -> str:
        return f"Tank {self.name + ' ' if self.name is not None else ''}[{', '.join(f'{100 * p:.3f}%' for p in self.percentages)}]{' X'*self.oxydized}"



class Node:
    index = 0
    def __init__(
            self,
            tanks: list[Tank],
            connections: list[tuple[int, int]],
            parent: "Node | None" = None,
            step: int = 0,
            n_connections: int = 0
            ) -> None:
        self.tanks = tanks
        self.connections = connections
        self.parent = parent
        self.step = step
        self.n_connections = n_connections
        self.index = Node.index
        Node.index += 1

    @property
    def fulls(self) -> list[Tank]: # Iterator
        return [tank for tank in self.tanks if tank.is_full()]

    def closeness(self, target: list[float]) -> float:
        if not self.fulls or not target:
            return float("inf")
        if self.difference(target) < PERCENTAGE_LIMIT:
            return 0
        return round(min(
            tank.closeness(target)
            for tank in self.fulls
        ), 5)

    def best_tank(self, target: list[float]) -> Tank:
        if not target:
            raise ValueError("Cannot find the best tank if there is no recipe")
        return min(self.fulls, key=lambda tank: tank.difference(target))
    
    def difference(self, target: list[float]) -> float:
        if not self.fulls:
            return float("inf")
        return sum(tank.difference(target) for tank in self.fulls)
    
    def wasted(self) -> float:
        return round(sum(tank.content for tank in self.tanks if tank.oxydized), 6)
    
    def wasted_final(self) -> float:
        return round(sum(tank.content for tank in self.tanks if tank.oxydized or (tank.is_not_full() and tank.is_not_empty())), 6)
    
    def unused(self, target: list[float]) -> float:
        return round(sum(tank.size for tank in self.tanks if tank != self.best_tank(target) and not isclose(max(tank.percentages), 1)), 6)
    
    def last_improvement(self, target: list[float], comparer: "Comparer") -> int:
        if self.parent is None:
            return 0
        if comparer(self.parent, target) > comparer(self, target):
            return 0
        return self.parent.last_improvement(target, comparer) + 1
    
    def produced(self, target: list[float]) -> float:
        if not self.fulls:
            return 0
        volume = 0
        formula = self.best_tank(target).percentages
        for tank in self.tanks:
            if tank.oxydized:
                continue
            if all(isclose(v1, v2) for v1, v2 in zip(tank.percentages, formula)):
                volume += tank.content
        return volume

    def transfer(self, connections: list[tuple[int, int]]) -> "Node | None":
        # Calculate the tranfer speed from/into each tank
        remaining_connections = connections.copy()
        tanks = [tank.copy() for tank in self.tanks]
        in_tanks, out_tanks = zip(*connections)
        extract_speed = {tank_idx: in_tanks.count(tank_idx) for tank_idx in set(in_tanks)}
        insert_speed = {tank_idx: out_tanks.count(tank_idx) for tank_idx in set(out_tanks)}

        # Dividing the transfert in smaller steps
        while any(insert_speed.values()):  # Still pumping, keep going
            # Calculate how much we will transfert at this step
            volumes_extract = [
                tanks[tank_idx].content / speed
                for tank_idx, speed in extract_speed.items()
            ]
            volumes_insert = [
                (tanks[tank_idx].size - tanks[tank_idx].content) / speed
                for tank_idx, speed in insert_speed.items()
            ]
            transfert_volume = min(*volumes_extract, *volumes_insert)

            # Change the content in each tank accordingly
            for a, b in remaining_connections:
                tank_a, tank_b = tanks[a], tanks[b]
                factor = transfert_volume / tank_a.content
                for i in range(len(tank_a.percentages)):
                    t = tank_a.percentages[i] * tank_a.size * factor
                    tank_a.percentages[i] -= t / tank_a.size
                    tank_b.percentages[i] += t / tank_b.size

            # Remove unnecessary connections and change speed
            for connection in remaining_connections.copy():
                a, b = connection
                if tanks[a].is_not_empty() and round(tanks[b].content, 6) < tanks[b].size:
                    # Still need this connection
                    continue
                remaining_connections.remove(connection)
                extract_speed[a] -= 1
                insert_speed[b] -= 1
                if extract_speed[a] == 0:
                    in_tanks = list(filter(lambda i: i != a, in_tanks))
                    extract_speed.pop(a)
                if insert_speed[b] == 0:
                    out_tanks = list(filter(lambda j: j != b, out_tanks))
                    insert_speed.pop(b)

        for tank in tanks:
            if isclose(tank.content, 0):
                tank.oxydized = False # Tank gets cleaned
                continue
            if isclose(tank.content, tank.size):
                continue
            # Tank is not completely full/empty
            tank.oxydized = True

        return Node(tanks, connections, self, self.step + 1, self.n_connections + len(connections))

    def children(self) -> "list[Node]":
        # Split the tanks into possible inputs and outputs
        possible_in: list[int] = [i for i, tank in enumerate(self.tanks) if tank.is_full() and not tank.oxydized]
        possible_out: list[int] = [i for i, tank in enumerate(self.tanks) if tank.is_not_full()]

        # Validate connections
        combinations: list[list[tuple[int, int]]] = []
        for connections in pairs_combinations_generator(possible_in, possible_out):
            if not connections:
                # Needs at least one connection
                continue
            xs = [pair[0] for pair in connections]
            ys = [pair[1] for pair in connections]

            too_many = False
            for x in set(xs):
                if xs.count(x) > CONNECTIONS_LIMIT:
                    too_many = True
                    break
            for y in set(ys):
                if ys.count(y) > CONNECTIONS_LIMIT:
                    too_many = True
                    break
            if too_many:
                continue # Cannot have a tank connected to too many others

            # if sum(self.tanks[i].content for i in set(xs)) < sum(self.tanks[i].size - self.tanks[i].content for i in set(ys)):
            #     # Output must be smaller than the input
            #     continue
            combinations.append(connections)

        # Generate all the children from the possible connections
        children: list[Node] = []
        for connections in combinations:
            child = self.transfer(connections)
            if child is not None:
                children.append(child)
        return children

    def __eq__(self, other: object) -> bool:
        if not isinstance(other, Node):
            return False
        return all(self_tank == other_tank for self_tank, other_tank in zip(self.tanks, other.tanks))

    def __hash__(self) -> int:
        return hash(tuple(tuple(tank.percentages) for tank in self.tanks))

    def __repr__(self) -> str:
        return f"Step {self.step}: [\n\t" + "\n\t".join(repr(tank) for tank in self.tanks) + "\n]"
    


class MCTSNode:
    def __init__(self, origin: Node, parent: "MCTSNode | None") -> None:
        self.origin = origin
        self.parent = parent
        self.remaining = list(origin.children())
        self.children: list[MCTSNode] = []
        self.score: float = 0
        self.n = 0
    
    def run(self, limit: int, is_done: Callable[[Node], bool], get_score: Callable[[Node], float]) -> Node:
        for i in range(limit):
            # print("Iteration", i)
            leaf = self.select_expand(MCTSNode.UCB1)
            node = self.rollout(leaf.origin, is_done)
            score = get_score(node)
            leaf.backpropagation(score)
        return self.best()
    
    def select_expand(self, ucb1: "Callable[[MCTSNode], float]") -> "MCTSNode":
        node = self
        while node.children and not node.remaining:
            node = max(node.children, key=ucb1)
        if not node.remaining:
            return node # There are no children at all
        leaf_node = node.remaining.pop(0)
        leaf = MCTSNode(leaf_node, node)
        node.children.append(leaf)
        return leaf

    def rollout(self, node: Node, is_done: Callable[[Node], bool]) -> Node:
        while not is_done(node):
            possibles = node.children()
            if not possibles:
                return node
            node = choice(possibles)
        return node
    
    def backpropagation(self, score: float) -> None:
        node = self
        while node is not None:
            node.score += score
            node.n += 1
            node = node.parent
    
    def best(self) -> Node:
        if not self.children:
            return self.origin
        return max(self.children, key=lambda node: node.n).origin

    def UCB1(self):
        w_i = self.score
        n_i = self.n
        N_i = self.parent.n
        C = math.sqrt(2)
        return (w_i / n_i) + C * math.sqrt(math.log(N_i) / n_i)



# Note: tuples can be compared in python, comparing the first elements, and the following ones if necessary
Comparer = Callable[[Node, list[float]], tuple[Any, ...]]
DEFAULT_COMPARER: Comparer = lambda node, recipe: (
    node.closeness(recipe), # First sort by closeness to the recipe
    -round(node.produced(recipe), 6), # Then by the amount of unused
    round(node.wasted_final(), 6), # Then by the amount of waste
    node.step, # Then by number of steps
    node.n_connections, # Then by the number of connections
    tuple(node.connections) # Finally for the test cases, when multiple tanks have the same content, use the first one
)



def load(recipe_path: str, volumes_path: str) -> tuple[list[float], list[float]]:
    # Read the recipe and volumes from files
    with open(recipe_path) as f:
        recipe = [float(line) for line in f.readlines() if line]
    with open(volumes_path) as f:
        volumes = [float(line) for line in f.readlines() if line]

    ## Optionally, check for the recipe to amount to 100% (will be normalized anyway)
    # if sum(recipe) != 1:
    #     raise ValueError(f"The recipe must be a total of 100%, not {100*sum(recipe)}%")

    return recipe, volumes


def normalize(l: list[float], factor: float = 1, rounding: int | None = None) -> list[float]:
    s = sum(l)
    if not s:
        return l
    if rounding is None:
        return [x * factor / s for x in l]
    if rounding:
        return [round(x * factor / s, rounding) for x in l]
    return [int(round(x * factor / s, rounding)) for x in l]


def pairs_combinations_generator(l1: list[int], l2: list[int], stack: list[tuple[int, int]] | None = None):
    if stack is None:
        stack = []

    # Generator for all the possible combinations
    # TODO: Change order of yielding [[(1, 1), (1, 2), (1, 3), ...], ...] -> [[(1, 1)], [(1, 2)], [(1, 3)], ...]
    yield stack
    if len(stack) == len(l1) * len(l2):
        return
    # m is the last pair checked, used for ordering and thus removing duplicates
    m = max(stack, default=(-float("inf"), 0))
    for a in l1:
        for b in l2:
            pair = (a, b)
            # Thanks to https://docs.python.org/3/library/stdtypes.html#common-sequence-operations:~:text=tuples%20and%20,elements
            if pair <= m:
                continue  # Case already checked
            # TODO: Flatten from recursive to iterative to prevent RecursionError
            yield from pairs_combinations_generator(l1, l2, stack+[pair])


def solve(recipe: list[float], volumes: list[float], comparer: Comparer, depth_limit: int = 10, percentage_limit: float = 0.01) -> Node:
    # Initialization
    temp_time = time()
    recipe = normalize(recipe)
    start = Node([
        Tank(
            volumes[i],
            [int(i == j) for j in range(len(recipe))]
        )
        for i in range(len(volumes))
    ], [])
    if not recipe or start.difference(recipe) < percentage_limit:
        return start
    node, parent = start, None

    # We limit the depth at which we check
    limit = 10_000
    for depth in range(depth_limit):
        print("Step", depth)
        mcts = MCTSNode(node, parent)
        # Recreate the functions because the best one changed
        def get_score(node: Node) -> float:
            # return -node.closeness(recipe)
            return node.closeness(recipe) < mcts.origin.closeness(recipe)
        def is_done(node: Node) -> bool:
            return node.step >= depth_limit or node.difference(recipe) < percentage_limit
        child = mcts.run(limit, is_done, get_score)
        # TODO: If child is worse, stop here
        node, parent = child, mcts

    return node


def bruteforce(recipe: list[float], volumes: list[float], comparer: Comparer, depth_limit: int = 10, percentage_limit: float = 0.01) -> Node:
    # Initialization
    temp_time = time() # TEMP
    recipe = normalize(recipe)
    start = Node([
        Tank(
            volumes[i],
            [int(i == j) for j in range(len(recipe))]
        )
        for i in range(len(volumes))
    ], [])
    if not recipe or start.difference(recipe) < percentage_limit:
        return start
    best_node = start
    best_score = comparer(start, recipe)
    children: list[Node] = [start]
    done: set[Node] = set() # TODO: Storing all nodes takes a lot of memory (and time to iterate), can we remove it?

    # We limit the depth at which we check
    for depth in range(depth_limit):
        if not children:
            break # We checked every possible combination
        # print(f"Step {depth}: {len(done)} done, {len(children)} this iteration")
        print(f"Step {depth}: {len(done)} {len(children)} this iteration")
        parents, children = children, []
        done.update(parents)

        # Generate all the children
        for parent in sorted(parents, key=lambda node: node.connections):
            for child in parent.children():
                closeness = child.closeness(recipe)
                if child in done or not isfinite(closeness) or closeness == 0:
                    continue
                if child.last_improvement(recipe, comparer) >= MAX_NO_IMPROVEMENT:
                    continue
                children.append(child)
                # print(child, child.connections, parent.connections)

                # Memorize the best one yet
                score = comparer(child, recipe)
                if score < best_score:
                    best_score, best_node = score, child
        
        # TODO: Check if solution is still valid afterwards
        if best_node.closeness(recipe) == 0 and best_node.unused(recipe) == 0:
            return best_node # No need to go further, you won't find better

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


def to_dot_graphviz(node: Node, recipe: list[float], rounding: int = 3):
    import tempfile # TEMP
    with tempfile.NamedTemporaryFile("w+", encoding="utf-8") as f: # TEMP
        f.write(to_dot_text(node, recipe, rounding)) # TEMP
        f.seek(0)
        import pygraphviz as pgv
        G = pgv.AGraph(f.name) # TEMP
        # TODO
        G.layout("dot")
        return G

def to_dot_text(node: Node, recipe: list[float], rounding: int = 3) -> str:
    # Convert formulas to text
    T, S = len(node.tanks), node.step
    recipe = normalize(recipe)
    F = node.best_tank(recipe).percentages
    best_formula = normalize(F, 100, 0)
    expected = " + ".join(
        f"{percentage}% N°{j + 1}"
        for j, percentage in enumerate(normalize(recipe, 100, 0))
    )
    actual = " + ".join(
        f"{percentage}% N°{j + 1}"
        for j, percentage in enumerate(best_formula)
    )

    # Start & formula nodes
    lines = [
        'digraph "blending steps\" {',
        'node [shape=box]',
        f'expected [label="Expected: {expected}"]',
        f'actual [label="Actual: {actual}"]',
        '',
        'node [shape=none fixedsize=true width=1 label=""]',
        '_ [group=0]'
    ]

    # Add labels for the tanks and steps axis
    for i, tank in enumerate(node.tanks):
        i += 1 # First tank is N°1
        lines.append(f'T{i} [group={i} label="Tank {i}\\n{round(tank.size, rounding)}"]')
    lines.append('S0 [group=0 label="Start"]') # FIXME: If no steps, then there is double S0
    for s in range(1, S):
        lines.append(f'S{s} [group=0 label="Step {s}"]')
    lines.append(f'S{S} [group=0 label="Final"]')

    # Add tanks and connections
    lines += ['', 'node [shape=box]']
    connections = []
    node_ = node
    step = S
    factor = min(tank.size for tank in node.tanks if tank.size) or 1
    while step >= 0 and node_ is not None:

        # Generate the tanks
        for i, tank in enumerate(node_.tanks):
            i += 1
            other = ''
            # Get the content of each tank
            wines = []
            for j, percentage in enumerate(tank.percentages):
                if isclose(percentage, 0):
                    continue
                wines.append(f'N°{j + 1}: {round(percentage * tank.size, rounding)}')
            # Additional display info for the content or the waste
            if wines:
                other = ' label="' + '\\n'.join(wines) + '"'
            if step == S and tank.is_full() and all(isclose(a, b) for a, b in zip(F, tank.percentages)): # TODO: pass as arg
                other += ' color=green fontcolor=green'
            elif tank.oxydized or (step == S and tank.is_not_full() and tank.is_not_empty()):
                other += ' color=red fontcolor=red'
            # Format and add
            lines.append(f'T{i}S{step} [group={i} height={tank.size / factor}{other}]')

        # Generate the connections
        for origin, destination in node_.connections:
            connections.append(f'T{origin + 1}S{step - 1}:s -> T{destination + 1}S{step}:n')

        node_ = node_.parent
        step -= 1
    
    lines += ['', 'splines=line']
    lines += connections

    # Add invisible edges to make a grid
    lines += [
        '',
        'edge [style=invis]',
        f'expected:s -> {{_:n T{T}}}',
        f'{{S{S}:s T{T}S{S}}} -> actual:n',
        f'{{ rank=same; _ -> T{" -> T".join(str(i) for i in range(1, T + 1))} }}'
        f'_ -> S{" -> S".join(str(s) for s in range(S + 1))}'
    ]
    for i in range(1, T + 1):
        line = f'T{i}'
        for s in range(S + 1):
            line += f' -> T{i}S{s}'
        lines.append(line)
    for s in range(S + 1):
        line = f'{{ rank=same; S{s}'
        for i in range(1, T + 1):
            line += f' -> T{i}S{s}'
        line += ' }'
        lines.append(line)

    # Group it all
    return "\n\t".join(lines) + "\n}"


def run_unit_tests(tests: list[dict[str, str | list[float]]]) -> None:
    for i, test in enumerate(tests):
        # Load test data
        try:
            name = test["name"] if "name" in test else f"TC{i + 1}"
            recipe = normalize(test["recipe"])
            volumes = test["volumes"]
            expected_formula = normalize(test["formula"])
            expected_steps = test["steps"]
        except KeyError as e:
            raise ValueError(f"Unit test n°{i + 1} build incorrectly") from e

        # Check test
        try:
            solution = solve(recipe, volumes, DEFAULT_COMPARER, DEPTH_LIMIT, PERCENTAGE_LIMIT)

            expected_formula = normalize(expected_formula, rounding=3)
            if recipe and all(solution.tanks):
                actual_formula = normalize(solution.best_tank(recipe).percentages, rounding=3)
                assert actual_formula == expected_formula, f"Expected {expected_formula}, got {actual_formula}"

            actual_steps = generate_steps(solution)
            assert actual_steps == expected_steps, f"Expected {expected_steps!r}, got {actual_steps!r}"
        except Exception as e:
            raise RuntimeError(f"Test '{name}' failed") from e
    print("All tests ran successfully")
    print()


def main():
    # TODO: argparse
    # import argparse
    # parser = argparse.ArgumentParser(
    #     prog="Wine Mixer",
    #     description="Wine mixing steps generator",
    # )
    # TODO: Find explicit names
    # parser.add_argument("recipe")
    # parser.add_argument("volumes")
    # parser.add_argument("") # TODO: Default to stdout
    # parser.add_argument("") # TODO: Default to stdout

    recipe, volumes = load("test/recipe.txt", "test/tanks.txt") # TODO: paths from args

    import random
    # recipe = normalize([random.randrange(5, 51) for _ in range(3)], rounding=4)
    # volumes = [5*random.randrange(2, 11) for _ in range(5)]

    if len(recipe) > len(volumes):
        raise ValueError(f"Cannot store {len(recipe)} wines in only {len(volumes)} tanks")
    normalized_recipe = normalize(recipe)
    algo = bruteforce # TODO: algo from args



    # print("Expected:", recipe)
    # print("Tank sizes:", volumes)

    # print()
    # start_time = time()
    # solution = algo(recipe, volumes, DEFAULT_COMPARER, DEPTH_LIMIT, PERCENTAGE_LIMIT) # TODO: config file / args
    # end_time = time()
    # print(f"Took: {end_time - start_time:.3f} seconds")

    # if solution.tanks:
    #     best_tank = solution.best_tank(normalized_recipe)
    #     index = solution.tanks.index(best_tank) + 1
    #     output_formula = normalize(best_tank.percentages, factor=sum(recipe), rounding=3)
    #     output_volumes = normalize(best_tank.percentages, factor=best_tank.size, rounding=3)
    #     save(solution, best_tank.percentages, "test/formula.txt", "test/steps.txt") # TODO: paths from args
    # else:
    #     best_tank = None
    #     index = None
    #     output_formula = []
    #     output_volumes = []
    #     save(solution, [], "test/formula.txt", "test/steps.txt") # TODO: paths from args
    # step, node = -1, solution
    # while node is not None:
    #     step, node = step + 1, node.parent
    # steps = generate_steps(solution)

    # print()
    # print("Best formula found in tank", index, "after", step, "steps.")
    # print("The formula is:", output_formula)
    # print("With the following volumes:", output_volumes)
    # print("Total volume generated:", sum(output_volumes))
    # print("Wasted volume:", solution.wasted())
    # print("Unused volume:", solution.unused(normalized_recipe))

    # print()
    # print("Steps:")
    # print(steps)





    recipe = normalize(recipe)
    solution = Node([
        Tank(
            volumes[i],
            [int(i == j) for j in range(len(recipe))]
        )
        for i in range(len(volumes))
    ], [])
    childrens = [469, 319, 459, 469]
    for i in childrens:
        solution = solution.children()[i]
    # for i, node in enumerate(solution.children()):
    #     print(i, node.connections)

    # graph_txt = to_dot_text(solution, recipe)
    # with open("graph.dot", "w", encoding="utf-8") as f: # TODO: paths from args
    #     f.write(graph_txt)
    G = to_dot_graphviz(solution, recipe)
    G.draw("test/graph.png")



if RUN_TESTS:
    run_unit_tests(UNIT_TESTS)

if __name__ == "__main__":
    main()


T 1:11 0:28 1:33 = 3:12
Q 1:22 1:47      = 3:09
L 3:19           = 3:19

4: