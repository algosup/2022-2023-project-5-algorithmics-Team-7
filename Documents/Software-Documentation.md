# Krug Champagne Project
##  Software Documentation

<details>
<summary>Table of contents</summary>

- [Krug Champagne Project](#krug-champagne-project)
  - [Software Documentation](#software-documentation)
  - [1. How to download and open the software?](#1-how-to-download-and-open-the-software)
  - [2. Input](#2-input)
    - [a. Recipe](#a-recipe)
    - [b. Tanks](#b-tanks)
  - [3. Usage](#3-usage)
    - [a. Windows](#a-windows)
    - [b. Mac](#b-mac)
  - [4. Output](#4-output)
</details>

## 1. How to download and open the software?

Go to the [2022-2023-project-5-algorithmics-Team-7](https://github.com/algosup/2022-2023-project-5-algorithmics-Team-7) repository and download WineMixer.zip.
Then, unzip the file and follow the instructions below.

## 2. Input

### a. Recipe

Recipe.txt will contain the wanted recipe with percentages of starting wines.
e.g. If the wanted champaign contains 50% of wine A, 30% of wine B and 20% of wine C, the file will contain:

```txt
0.50
0.30
0.20
```

### b. Tanks

Tanks.txt will contain the list of tanks with their sizes. It's important to respect the order of the tanks. To respect the formula you put in Recipe.txt, the first tank will be the first one you put in Recipe.txt, the second tank will be the second one you put in Recipe.txt, etc. The unit of the size doesn't matter as long as you respect the same unit for all the tanks. e.g. If the first tank (wine 1) is 1000L, the second one (wine 2) is 500L and the third one (wine 3) is 2000L, the fourth one (empty) is 1000L, the fifth one (empty) is 500L and the sixth one (empty) is 2000L, the file will contain:

```txt
1000
500
2000
1000
500
2000
```

## 3. Usage

When the inputs files are correctly filled, use the following commands to run the program.

### a. Windows

Open the console, and type:
    
```bash
cd Downloads/WineMixer
```

Then type:

```bash
WineMixer.exe
```

### b. Mac

Open the console, and type:
    
```bash
cd Downloads/WineMixer
```

Then type:

```bash
sudo dotnet WineMixer.dll input/tanks.txt input/recipe.txt && open /usr/local/share/dotnet/output
```

## 4. Output

In the folder WineMixer, you will find a directory called output. In this directory, you will find two files:
-  The first one is called "result.txt" and is similarly written to "recipe.txt" (the actual percentage of each wine in the final champaign). 
- The second one is called "steps.txt" and contains the steps (from one tank to another) to follow to obtain the final champaign.

e.g. If the recipe is 50% of wine A, and 50% of wine B, the capacity of tank 1 is 2000L, the capacity of tank 2 is 1000L, the capacity of tank 3 is 1000L, and the capacity of tank 4 is 1000L, the files will contain:

result.txt:
```txt
0.5
0.5
```

steps.txt:
```txt
(1) -> (3,4)
(2,3) -> (1)
```

If you are on windows, you will find these files in the folder WineMixer/output.

If you are on MacOs you will find it following this path: /usr/local/share/dotnet/output