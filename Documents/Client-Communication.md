## The Tuesday 02 May 2023
Hello Franck, our team and I have a few questions regarding the start of this project.
Regarding client contact, could you please clarify who will be our main point of contact with Krug Champagne during this project? And what is the best way to reach out to them if we have any questions or need additional information?
For the formula of the champagne what is the percentage breakdown of each wine in the final blend? And what is the specific formula ?
I would also like to know more about the various tank sizes that we have at our disposal. Can you please provide a list of the different tank sizes we have and the capacity of each tank?
Thank you.

Hello Thomas, I'm going to be the customer contact for the time being. The formula varies from year to year and is an input of your software. You have to design and provide a solution for the customer to be able to input the values. It will be something like 1.2%wine1+2.7%wine2+etc.
Tanks come in all shapes and sizes. Again, nothing should be hard-coded in your software. You could have a configuration file, an API etc. you have to make proposals at the design stage and have them agreed upon by the customer.



## The Wednesday 03 May 2023
Karine Vinette <br>
  14 h 32 <br>
Hello,
As you know, it is crucial that the wine is not exposed to oxygen during the blending process. In order to ensure that the wine remains free from oxidation, it is important to fill each tank completely or leave it completely empty, rather than partially filled.
Could you please provide us with more information on how to deal with partially filled tanks? Specifically, how should we proceed if we have wine left over after filling one tank and need to fill another tank with a smaller amount? Is it possible to transfer the remaining wine to a smaller tank, or should we discard it to avoid exposure to oxygen?
14 h 35
As we begin the blending process for the Krug Champagne project, I wanted to confirm the availability of tanks for the blending process. Could you please let me know if the tanks are currently empty and ready to be filled with the respective wines?
Additionally, I wanted to inquire about the current location of the wines we will be using for the blending process. Could you provide me with an update on where they are stored and the condition they are in? This information will help us ensure that we can maintain the quality of the wine during the blending process.


Karine Vinette <br>
  14 h 53 <br>
Could you please provide us with the volume of the tanks that will be used for blending? This information will help us determine the optimal number of steps required to achieve the desired results while minimizing waste.
Additionally, it would be helpful to know the volume of the wines that will be used in the blending process or that we have access. This will allow us to calculate the ratios required to achieve the desired recipe.
We would appreciate any information you can provide on the expected amount of production and the specific recipe that will be used for the blending process. This information will allow us to tailor the software to your specific needs and ensure that we are able to deliver the results you are looking for. (modifié) 


Karine Vinette <br>
  15 h 12 <br>
I wanted to ask a question regarding the champagne blending process. When filling three tanks at the same time, is it considered as one step or three separate steps? I appreciate your help in clarifying this matter.


Franck JEANNIN <br>
  17 h 02 <br>
Your software should be generic and work regardless of how many tanks there are and how big they are. At the start, wine 1 is in tank 1, wine 2 is in tank 2, etc. plus you have a number of empty tanks of various sizes.
You simple cannot leave a tank half empty/full. In other words if you are mixing 3 tanks of 500 hectoliters they have to go into a 1500 hectoliters tanks, there is no other solution.
1,2 -> 3,4,5 (tank 1 plus 2 going to 3,4 and 5) is one step.
Then you could have 3,4 -> 1 (you can reuse empty tanks if they are of the right size).
