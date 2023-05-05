<hr>

<p align="center" style="font-weight: bold; font-size: 21px"> Krug Champagne Blending Software</p>
<p align="center" style="font-weight: bold; font-size: 18px"> Functional Specifications</p>
<br>
<p align="center"> Karine VINETTE</p>
<br>

<p align="center"> ALGOSUP, Group 7. All Rights Reserved. </p>

<hr>

<details>
<summary>Table of contents</summary>

- [Overview](#overview)
- [Scope](#scope)
- [Requirements](#requirements)
  - [Functional requirements](#functional-requirements)
  - [Non-functional requirements](#non-functional-requirements)
- [Use cases](#use-cases)
- [Data requirements](#data-requirements)
- [System architecture](#system-architecture)
- [Assumptions and dependencies](#assumptions-and-dependencies)
- [Acceptance criteria](#acceptance-criteria)
- [Glossary](#glossary)

</details>


# Overview

The goal of this project is to build software that can blend large quantities of wine in the right proportions to produce the closest result to the formula with the minimum number of steps. The software will focus on stage 4: Blending, and it will be used by the Cellar Master, Julie CAVIL, and her team.

# Scope

The scope of the project would be to create a software system that can assist in the blending process of Krug Champagne's new winery. The system should allow the Cellar Master and her team to input the desired formula for the champagne blend and then automatically calculate the optimal combinations of the wines in the various tanks to achieve the desired result. The system should also ensure that the tanks are never left partially full to avoid oxidation and contamination of the wine.

The system should be able to handle a large number of tanks and blends, and it should be designed to scale up or down as needed. The system should also be able to track the history of each tank, including the wine that has been stored in it, the dates it was filled and emptied, and any other relevant information.

In addition, the system should provide real-time feedback to the Cellar Master and her team as they adjust the blend, allowing them to see how changes in the proportions of different wines will affect the final result. The system should be user-friendly and intuitive, allowing the Cellar Master and her team to easily navigate and use its features.

Overall, the scope of the project is to create a comprehensive software system that streamlines the blending process, reduces errors, and ensures consistency in the final product.

# Requirements

The software should be able to blend wine from 330 tanks of various sizes in the right proportions to produce the closest result to the input formula.

The software should ensure that no tank is left partially full or partially empty due to the risk of oxidation.

The software should be written in a language that is fast and efficient.

The software should be able to produce the final product in the minimum number of steps possible.

The software should be able to produce the closest result to the input formula.

## Functional requirements

The software should take as input a text or read from a text file with the following format:
```
20   40 40
12.5 25 37.5
18.75 18.75 43.75
```
Here is are more details:
- The first line contains the desired formula with the percentages for each wine
- The second line has the volume of the tanks containing the wines
- The third line has the volume of the additional tanks for blending
- The spacing to align the first and second lines is pure syntactic sugar, thus long number may ne be broken down
- To accomodate all kinds of representations, decimal separators may be either a dot `.` or a comma `,`
- The program should give the user an error message if the formula does not add up to 100% or if the number of wines in first two lines does not match
- *If time allows it, the optional usage of volume units may be added*

The software should return a block of text containing the following informations:
- The closest resulting formula from the required one and the steps to achieve it
- *If time allows it, the formula generating the less waste and the steps to achieve it*
- *If time allows it, a formula that consiliates conformity of the desired formula and steps to achieve it*
- The values should be given with up to 3 decimals
- The number in parenthesis is the volume of that wine *with the provided unit if any*
```
Closest match:
N°1: 19.048% (8.333)
N°2: 38.095% (16.667)
N°3: 42.857% (18.75)
Step 1: 3 -> 4 & 5 (N°3)
Step 2: 1 & 2 -> 3 (N°1 & N°2)
Step 3: 3 -> 1 (N°1 & N°2), 3 & 5 -> 6 (N°1-3)
Result in tank(s): 6
Remain in 1: N°1 & N°2
Remain in 4: N°3

Fewer waste:
N°1: ...
```

(Here is the same example but visualized:)
![Example](documents/example.png)

The software should have a user-friendly interface that allows the Cellar Master and her team to input the formula and specify the quantities of wine to be blended.

The software should be able to access data on the tanks, their sizes, and their current contents.

The software should be able to calculate the best combination of tanks to use based on the input formula and the available wine in the tanks.

The software should be able to track the quantity of wine being pumped from each tank and ensure that no tank is left partially full or partially empty.

The software should be able to adjust the blending process in real-time if any tanks become unavailable or if the input formula changes.

The software should be able to generate a report on the blending process, including the number of steps taken, the quantity of wine used from each tank, and the final product's characteristics.

## Non-functional requirements

The software should be highly secure to prevent unauthorized access to the system.

The software should be highly reliable to ensure that there are no crashes or errors during the blending process.

The software should be scalable to handle a large number of tanks and blending combinations.

The software should be written in a language that is easy to maintain and update.

The software should be highly performant to ensure that the blending process is completed in a reasonable amount of time.

# Use cases

Create a new blend: The Cellar Master or another authorized user can create a new blend by selecting the desired wines from the available inventory and specifying their proportions. The software should ensure that the selected tanks are completely full and not in contact with oxygen.

Modify an existing blend: The Cellar Master can modify an existing blend by adjusting the proportions of the wines used. The software should ensure that the modified tanks are completely full and not in contact with oxygen.

View inventory: The authorized users can view the current inventory of available wines in the tanks, including their quantities and properties such as grape variety, age, and origin.

View blend history: The authorized users can view the history of the blends created, including their formulas, proportions, and dates.

Export blend recipe: The authorized users can export the formula and proportions of a blend to a file or print it for record keeping or sharing with other parties.

Import blend recipe: The authorized users can import a blend formula and proportions from a file or external source, and the software should validate the input and ensure that the required wines are available in the inventory.

Monitor wine quality: The software can monitor the quality of the wine in the tanks, such as temperature, acidity, and pH, and alert the users if any parameter is outside the desired range.

Manage tanks and pipes: The authorized users can manage the tanks and pipes, including adding or removing tanks, connecting or disconnecting pipes, and cleaning or sterilizing the equipment.

# Data requirements

Grape data: information about the different grape varieties used in making the champagne, including their characteristics and harvest dates.

Tank data: information about the various tanks available for blending, including their sizes, contents, and previous use history.

Blending data: data on the different blends created during the testing phase, including their ingredients and proportions, as well as notes from the tasting team.

Formula data: the final formula for the Krug Grande cuvée, including the ingredients and proportions.

Production data: records of each batch produced, including the tanks used, their contents, and the blending process.

Quality control data: records of quality control tests performed on each batch, including measurements of alcohol content, pH levels, and other relevant parameters.

Inventory data: records of the current inventory levels for each grape variety, tank, and finished product.

Traceability data: records of the origin and history of each grape variety used in production, as well as the tanks and other equipment used.

All of this data will need to be stored in a central database that can be accessed by the blending software to ensure accurate and efficient production.

# System architecture

User Interface: The user interface (UI) is responsible for allowing the user to input the desired formula for the blend, as well as displaying the output results. This could be a web-based application or a desktop application.

Formula Database: The formula database stores the different combinations of wines that have been tested and approved by the Cellar Master and her team.

Tank Management System: The tank management system is responsible for keeping track of the wine stored in each tank, ensuring that tanks are never left partially full or empty, and that wine is not exposed to oxygen. This system should also be capable of connecting any tank with any other tank using a system of pumps and pipes.

Blending Engine: The blending engine is the core component of the system, responsible for calculating the optimal blend based on the input formula and the available wine stored in the tanks. It uses algorithms and data structures to find the minimum number of steps to produce the closest result to the input formula.

Data Analytics and Reporting: This component is responsible for analyzing data collected by the system and generating reports for the Cellar Master and her team. The reports could include information such as the number of times each tank has been used, the amount of wine remaining in each tank, and the optimal blend that was produced.

Security and Authorization: This component is responsible for ensuring that only authorized users can access the system and its data. It should include features such as authentication, authorization, and encryption to protect sensitive data.

Infrastructure: This component includes the hardware and software required to host the system. It could be hosted on-premises or in the cloud, depending on the requirements of the organization.

Overall, the system architecture should be scalable, flexible, and modular to accommodate future changes and updates to the software. It should also be designed with performance and reliability in mind to ensure that it can handle the complex calculations and data management tasks required for the blending process.

# Assumptions and dependencies

The tanks are reusable and can be used several times in the blending process.

The system of pumps and pipes can connect any tank with any other tank.

The input formula will not change during the blending process.

The software will not be used to blend wine from different years.

The software will only be used to blend wine during the second fermentation stage.

The software will not be used to age or store the wine.

The software will only be used by the Cellar Master and her team.

# Acceptance criteria

The software should be able to receive the wine blending formula from the Cellar Master, Julie CAVIL, as input.

The software should be able to access and manipulate data on the various tanks and their current contents.

The software should be able to calculate the optimal blending ratio of wines in the different tanks to achieve the closest possible result to the input formula.

The software should be able to output a detailed plan for blending the wines in the tanks to achieve the optimal ratio, including which tanks to use and in what order, and how much wine to transfer between the tanks.

The software should ensure that no tank is left partially full or empty at any point in the blending process to prevent oxidation of the wine.

The software should ensure that no step in the blending process causes any loss of wine.

The software should be able to track the progress of the blending process and provide real-time feedback to the user.

The software should ensure that the final product meets the input formula as closely as possible.

The software should be easy to use and understand, with clear and concise documentation provided.

The software should be efficient, with a minimum number of steps required to achieve the optimal blending ratio, and with a reasonable runtime.

# Glossary

Krug Champagne: a brand of champagne that is part of the LVMH group.

Winery: a facility where wine is made.

Méthode champenoise: a traditional method of making champagne that involves a complex process of fermentation and aging.

Pinot Noir: a black grape variety commonly used in champagne production.

Pinot Meunier: a black grape variety commonly used in champagne production.

Chardonnay: a white grape variety commonly used in champagne production.

Harvesting: the process of picking grapes from the vine.

Pressing: the process of extracting juice from the grapes.

Fermentation: the process by which yeast converts sugar into alcohol.

Still wine: wine that does not contain carbon dioxide bubbles.

Blending: the process of mixing different wines together to create a consistent flavor profile.

Second fermentation: the process by which carbon dioxide bubbles are created in champagne.

Lees: dead yeast cells that settle at the bottom of the bottle during aging.

Riddling: the process of gradually rotating and tilting champagne bottles to move the lees towards the neck of the bottle.

Disgorgement: the process of removing the frozen plug of lees from the neck of the bottle.

Dosage: the process of adding a mixture of wine and sugar to adjust the sweetness level of the champagne.

Cellar Master: the person responsible for managing the winemaking process.

Tanks: containers used for storing and blending wine.

Pumps and pipes: equipment used for transferring wine between tanks.

Oxidation: the process by which wine is exposed to oxygen, which can change its flavor and aroma.

Formula: the specific combination of wines and proportions used to create a desired flavor profile.

Idiomatic style: a programming style that is consistent with the conventions of the language being used.
