## Investigation

## First Attempt: Brute Force

The first thing attempted was a brute force space search of "operations".
After each operation a score is assigned to the state based on its 
distance from the desired blend. 

At each step, add wine to a tank, remove wine, or combine two tanks. 
This creates an infinite graph of possibilities, kind of like a chess game. 
At every stage we have a large number of possibilities which multiplied 
at every level. Kind of like the number of chess possibilities. 

After a few operations this number becomes astronomical. Consider 
if each operation has 20 possible next operations. 
After only 5 operations, the number of configurations to be considered 
is `20 * 20 * 20 * 20 * 20` which is 3,200,000. 

## Optimization: Precompute Combine Operation 

The first optimization applied was to precompute the set of 
valid "combine operations". There are n^2 possibilities to consider
where n is the number of tanks. The observation is that 
two tanks can only be combined if they add up in size to another tank. 

This optimization is important, but does not change the overall
running time of the algorithm. 

## Simplification: No Removing of Wine
 
Given the sheer size of the search space the ability to remove wine 
from a tank has been removed. 

## Second Attempt: Greedy Random Algorithm 

The next hypothesis is that the space was so large it wasn't searchable 
in completeness. Even though no discernible optimal sub-problem structure 
was uncovered in the algorithm, an attempt was made to choose random 
batches of states, choose a percentage of the best states, and apply 
random operations to them. 

For a small input tank set and input wine set this was able to 
generate some solutions that appeared reasonable. 

However, it was observed that the chances of such an algorithm 
getting stuck in a local minima were very high, and for it to 
virtually never consider possibilities that could 
lead to better results.  
	
## Optimization: Transitions instead of Operations

The tree created by looking at all operations could involve 
non-sensical operations like adding wine to a tank, and then 
never using it.  

In reconsidering the algorithm a bit like a person would operations 
were combined into groups that involved a combine and 0,1, or 2
tanks that have wine added. These groups are called transitions
and the requirement was that if wine is added.

This algorithm executed much faster, but still would not scale 
to the desired number of input tanks and possible wines. 

## The Problem Solving Approach

The problem solving approach used to this point was consistently:

1. Considered wines and tanks simultatneously - 
which multiplied the size of search space considerably 

2. Started from starting states and evaluating possible transitions.  

It was hypothesized that perhaps choosing wines could be done 
indpendently from considering tanks and the possible wine proportions 
that each one could create. 

Also it was hypothesized that we could work our way backwards 
starting from a tank and considering how it may be broken up 
into source tanks. 

## Thinking in Terms of Graphs 

Rather than thinking about wine up-front a new hypothesis 
was formed that one could just consider each tank and the 
possible combine operations to blend tanks. 

In effect this means we think about a graph where each node is a tank,
and each edge is a transition which itself can come from multiple tanks. 

This is not a standard way to think about graphs because 
an edge can have multiple inputs and a single output. 

What is interesting is that the graph space is not as enormous. 

For each tank we would arrive at a set of proportions representing the 
possible input tanks. For example consider the following tanks. 

```
A=10
B=5
	D=4
	E=1
C=5
	F=2
	G=3
H=6
```

Tank A can be fed by tanks B and C, or by tanks D and H. 

We observe that because B and C can be broken down, we can say that tank
A can be fed by tanks D, E, C or B, F, G. 

To simplify things we can just consider "source" nodes, i.e., those
with no inputs. 

In this case we can say that the possible sources of A are:

```
A <= D, E, F, G
A <= D, H
```

## Investigation

The major questions at this point are: 

1. how big will the search space be?  
2. is there repetition that we can removed? 
3. if so how can we remove the repetition? 
4. given a set of sources, how do we compare that to a wine blend? 

## From Tanks to Wine Blend 

A set of source can be represented as a list of integers, representing
the sizes. 

```
A <= 1, 2, 3, 4
A <= 4, 6
```

We can easily normalize to percentages: 

```
A <= 10, 20, 30, 40
A <= 40, 60
```

We observe that it is possible to put the same wine in two tanks. 
So we can also generate a large range of permutations: 

```
A <= 10, 20, 30, 40
A <= 10, 50, 40
A <= 10, 30, 60
A <= 10, 20, 70
A <= 10, 90
A <= 20, 40, 40
A <= 20, 30, 50
A <= 20, 80
A <= 30, 30, 40
A <= 30, 70
A <= 40, 60
```

## Unit Testing

Given that the main program had been written, and was basically working
the new hypotheses about tanks and graphs are created and tested
within a unit test project. 

## Observations

* If two tanks have the same size the input sources are the same. 
* The same input sources are generated over and over again. 
* It seems that the same core inputs are used over and over again. 
* The number of wines that can be used in a blend are restricted to the number of sources.
* The number of sources is not that large

### Challenge

* How do I quickly avoid having the same set of inputs. 

### Ideas:

* Once a tank size has been computed, re-use it for computing sources
* Maybe sources need to be stored outside of a node (dictionary)
* Maybe I can use a tree of some kind to represent the set of inputs?
* Alternatively I could use a hash set, and just has the list contents. 

### More observations

When using a hash set of tank sets, we can avoid 
significant amounts of recomputation. Computing the input 
sources was accomplished for a large set of tanks 
very quickly. 

Hashing is done on the string that represents the input 
source. 

The first attempt used tank indices. When multiple 
tanks have the same size, the result would be equivalent.

Therefore adjusted to output just sizes. 

## New Challenges: Tank Splitting and Multiple Inputs

In the recent investigation, tank splitting and multiple inputs were ignored 

When tanks can be split the process becomes more complicated.
The path is no longer straightforward, and now tanks can have wine
and no longer be available for consideration in the process. 

It is possible for tanks to have wine left over and unused. 

## Additional investigation

Is there a way of saying that one blending operation is better than another? 
That it brings us closer to the end goal? 

We could say simply: *does the blend result in a closer result?* However 
this does not take into the account that if we have four wines, combining 
A and B into AB, and C and D into CD, can be better than combining ABC.

What I mean is that they are kind of orthogonal to each other, but having a 
variety of wines brings us overall closer to the goal. 

## Hypotheses

* It seems logical that a variety of wines in different barrels is good. 
* Two wines that have the correct ratio to each other is attractive 
* However, the size of that tank is also an important consideration. 
* When tanks are used up, they are no longer available. 
* The wines with larger percentages matter more than the wines with smaller percentages. 

## Partial Optimal Subproblem

* Given two tanks of size X and having wines A and B
* Blending them should give a wine blend C that is closer to the target than either A or B. 

## New Observation: Vector Difference 

If we consider the blend of wine in a tank to be a vector. The goal is to 
get that wine as close to another. When combining wines, we are doing 
vector addition. So in effect we want to create a new wine blend that is 
a delta between the current best wine, and the target wine. 

So then at each stage we are maximizing for that. 

This means we no longer will get trapped in a local maxima. 

Some small questions remain: 
* what wine do I put in what tank first? 
* give two wine blends that exist, do I maximize A or B? 
* What if I create a wine blend that represents a delta, but it can't be added.

Putting the most desired wine (the one with the highest percentage) in the 
smallest tank first, doesn't make sense, because then you have to put the second most 
desired wine in the next larger tank, which throws off the proportions. 

## Looking at Tanks in Isolation versus Looking at Blend Possibilities

It seems pretty clear that looking at a single tank does not provide 
enough information. We almost want to just look at the possibility of blends. 

So to choose which wines to add, we need to start from the list of all possible 
blends that we can make, and figure out which one makes the most sense 
and is closest to the desired delta. 

So we can compute scores based on a difference from a delta. 

## Weighted Vector Difference (Lerp)

The subtraction hypothesis makes sense if the target wine and the new wine are 
blended in equal amounts. However, they can be blended in different amounts.

So for example: my best wine blend might be in tank A50L which can be blended
with tank B50L. However, it might also be able to be blended with tank B25L 
in which case the target vector would be half of the original.

## Scoring? 

So this raises a predicament vis-a-vis scoring of deltas. 
The purpose of scoring a delta is to help us construct new wines for blending. 
I can blend the delta into any tank I want: which one should I prefer. 

In general the scoring can help with choosing a wine, but not so much 
with choosing a tank. 

## Short Search then Greedy 

One idea is to do an exhaustive short search (e.g. two deep) of all of the possibilities and then choosing the best operation. 

We can then re-evaluate: maybe the next choice is a little different 
than we expect. This is like chess: we search as deep as we can, 
and continue to re-evaluate. 

Note that we know that vectors are additive. Two delta vectors can add up to 
become helpful. 

Looking at N tanks, some are going to be deltas. Is there something we can say 
about a delta? Does it look like a solution locally? 

## A Ray, not a Vector? 

If we think about a delta as a vector in 3 space, it can point away from 
the best solution. So evaluating it on its own vis a vis the target point 
doesn't make much sense. 

If we consider a delta as a ray (a point and a vector), then we know it  
points to the target wine. 

## Perfect Vectors 

At each state, we have a number of tanks. Each tank A (with wine) 
can be combined with another tank B (empty). Tank B has a theoreticaly 
perfect vector that is easily computed (via reverse interpolation)
which when combined with A would give us the perfect wine.  

So changing it around, every empty tank has a perfect vector for every 
occupied tank. 

## Idea: Throwing out Wine even if you cant. 

Assuming we can't (or don't want to) throw out wine, and that the problem doesn't 
have an optimal sub-problem, the greedy approach might still work , if we 
allow wine to be thrown out. We can then patch the operation path later (just
 don't ) add the wines that got removed. This might allows us additional 
 flexibility in traversing the solution space. 

## Perfect Vectors and Transitions 

Even if we know the perfect vectors for all the tanks, choosing 
what wine to add to what tank is still unclear. 

As mentioned before: looking at wines and tanks in isolation very likely won't work. 

A transition possibility:

* Add two wines to empty tanks. 
* Combine them into a new empty tank. 
* Now use that for one of the known "perfect vector". 

This can be evaluated for each transition, and we choose the transition that it 
closest to a perfect vector. 

What if we choose a transition, but don't apply it immediately. 
Instead we just do one operation (e.g. add the first wine)
This is like the idea of a short search, make best move re-evaluate. 

In theory at this point we are a little bit closer to making a better wine. 
We have a bit more of the desired wine in the system or we combined some wines to help. 

Maybe after making this step we look at the transitions again and we have 
a better idea. 

There is even a possibility that this always brings us to the best solution. 

## Alternative: Evaluating Two Transitions 

Would this not provide the same result as evaluating two transitions deep? 
Looking at the best wine, and not the delta vector? Or is it kind of like evaluating
 a transition + combine, and choosing that. 

The thing is that just looking at the wines in the system doesn't tell us about 
the wines we need. The hypothtesis is that the delta vector helps in choosing 
missing wines. 

This is not clear. 

## Side Note: Evaluating the Entire System (Graph)

Looking at the entire system and evaluating it is hard.

* Do we just look at the best wine blend? 
* Do we look at the difference vectors? 
* The more wines in the system the easier it is to make blends. 
* The fewer tanks used, the easier it is to make new blends  

## Perfect Deltas and Delta Deltas 

A perfect delta is a wine blend that when combined with an occupied tank 
will bring it directly to the desired wine. It is computed as the difference 
from the target blend to the blend in the occupied tank. 

Each occupied tank has a set of perfect deltas: one for each empty tank it can be 
combined with. The empty tank itself also a list of perfect deltas and the occupied 
tanks that it could be. 

Optionally: these deltas can be associated with the "combining" tanks. Those tanks then 
have two (or more) feeders. The delta   

In addition an empty tank can have one or more delta-deltas. A delta-delta 
is the vector required in an empty feeder tank to combine with a non-empty feeder tank 
to make the empty tank become a delta. 

This process could in theory continue until we get to the smallest tanks. 

## Looking at Distance from Target versus and Distance from Delta

Is there a difference between the distance from target and the distance from delta? 
It seems that minimizing distances is like minimizing error. The question is,
which one introduces the new best wine. 

## Experiment 

At this point an experiment will be done to:
* compute all possible transitions. 
* output how many there are at each step. 
* Allow serialization of a state (maybe?)
* choose an operation based on which transmission minimizes distance 
from a delta or target. 
* output the deltas
* output the distances of each state from the target. 

## Some Confusion about Euclidean Distance and Vector Length

If a wine blend is considered a vector in n dimensional space, the sum of the components 
must add up to one. This does not mean that the vector length is also one. 

This has lead to some confusion about how to compute scores. Is it correct that two 
wines are indeed closer in flavor profile is the euclidean distance is the same? 

In order to compare two vectors, we want the length of the distsance. To do this
the proportions must be normalized. However, a Euclidean normalization might not be 
appropriate. Instead we may want to simply divide by the sum of components. 

## Distances

Using Euclidean distance seems to still be a valid way forward. 

## Maximizing at Each Step

At each step we can either find:
1. the best wine mix, or 
2. the best delta. 

Finding a best delta only makes sense if we can get the delta into the final target. 

All things equal we would want the best wine in the smallest container. Maybe. 

## Normalization 

Just adding vectors isn't quite accurate, the result may have to be normalized.
Normalizing though has a problem because the proportions are no longer correct. 
The idea of delta vectors needs to be revisited. 

Trying to always normalize in advance causes a problem. We can't compute normal 
vectors etc. 

One possibility is to use proper amounts in each barrel. This would make 
computation a bit easier. The target for a barrel, is simply the percentage 
amount multiplied by the barrel size. 

## Normalizing the Distance

The idea of dividing by the sum doesn't work for vectors that are nearly zero
in length. When computing the delta we can get a vector that might have some 
arbitrary distance. 

We could compute distances to each target based on actual amounts: not normalized. 
This would be kind of interesting. 

This would require interpreting everything differently. 

## Hypothesis: Evaluating the Tree of Combines 

Given a state, how many combines are possible? At the most: N^2. 400 * 400 = 16000. 
This number will never arrive. Exploring the tree of all possible combines and evaluating
it for the best state would be one way to evaluate a position. 

So we could say, given a possible operation, what is the best possible outcome if 
all that happens afterwards are combines. Is this new tree better or worse? 

This is a large search space and it might get simpler. 

## Hypothesis: Average Mix of System and Distance

One hypothesis is that optimizing the system by computing the average mix of 
all of the tanks and computing that distance, will be an overall good thing. 

Either this can be the primary scoring metric, or it can be a secondary 
scoring metric. 

As a secondary metric, the first metric would be: how close is the best tank
to the overall final result. And given multiple tanks that meet that, 
whawt is the overall average. 

Another metric to be considered (maybe in between the two) is 
tank sizes. Or alternatively, how many tanks can it be combined with. 

## To measure 

How big is the search space? 

## Hypothesis:

Never make the best tank worse. In an ideal solution it is 
unlikely that making a worse wine, will ever help the final solution. 

Consider two options:
1. `AB = A @ B`
2. `AC = A @ C`

Where `@` represents the blend operation. What it means is a weighted average (Lerp)
between the  components for some weight T. 

Consider a score function `F(V) = Distance(B.Unit(), T.Unit())`.  

Consider two distances: 

The hypothesis is, if F(B) < F(C), then F(AB) < F(AC).

Try it with number.

```
T = 10. 
A = 5
B = 8
C = 6
```

It seems to work. 

## Transitions and Tanks Splits

When considering tank splits a problem can occur in that a tank combine and a tank split can 
be repeated causing an infinite loop. 

Two possible solutions:
* Never allow a previously visited solution.
* Force a tank split to be preceded/followed by adding some wine.

But why not force a tank combine to be preceded by adding some wine.

A split followed by another split followed by a combine, could make sense. 

## Some Notes

Adding wine when looking at the combine tree makes sense. Adding a new wine
should be done if it has the potential to improve things. 

All things equal: having the best wine in a smaller container is better. 












