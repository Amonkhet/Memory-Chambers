# Project 2 Report

Read the [project 2
specification](https://github.com/feit-comp30019/project-2-specification) for
details on what needs to be covered here. You may modify this template as you
see fit, but please keep the same general structure and headings.

Remember that you should maintain the Game Design Document (GDD) in the
`README.md` file (as discussed in the specification). We've provided a
placeholder for it [here](README.md).

## Table of Contents

- [Evaluation Plan](#evaluation-plan)
- [Evaluation Report](#evaluation-report)
- [Shaders and Special Effects](#shaders-and-special-effects)
- [Summary of Contributions](#summary-of-contributions)
- [References and External Resources](#references-and-external-resources)

## Evaluation Plan

**Evaluation techniques**: 
- Observational methods: think-aloud, post-task walkthroughs. 
- Querying technique: questionnaires.

We adopt think-aloud and post-task walkthroughs during observations. Because our game has elements of puzzle-solving, think-aloud method can reveal players’ reasoning paths in our puzzle pattern recognition, deduction, and cross-space logic. Think-aloud also exposes points of confusion early by hearing player's real-time expression. And it requires only screen/audio recording which is low setup cost. With the screen/audio recording, we will further conduct post-task walkthroughs to better understand players' reasoning. We will also ask questions to access users' overall emotional engagement and narrative comprehension of the game. With these two observational methods together, we want to encourage deeper verbalization without breaking too much immersion. For querting technique, we use questionnaires with ten fixed questions, each question scaling from 1 to 5 (Strongly disgree, disagree, neutral, agree, strongly agree). We will ask the players to play the game with the tasks to find the exit of the first room space; and to move the buildings in the second room space.

**Think-abloud & Post-task walkthrough: 3~5 questions**: 
- Why did you make certain movements (based on player's behaviours and observer's notes)?
- Why did you express certain feelings (based on player's emotions, confused, frustrated or excited)?
- How long did it take you to feel “comfortable” with how the game works?
- Describe any moments when the game did not behave as you expected.

**Questionnaire: 10 scaled questions**: (Strongly disgree, disagree, neutral, agree, strongly agree)
1. The character control and interactions were comfortable and intuitive.
2. The frame rate and animation felt smooth during play.
3. It was clear what I needed to do to progress or escape from the start.
4. I understood the time transition represented by the moon and building in the work space.
5. The layout of the environment felt logical and cohesive.
6. I understood the connection between the two rooms (“life” and “work”) and the larger space.
7. The visual (lighting, color, text) enhanced the experience of exploring.
8. The sound effects and/or background music enhanced the experience.
9. The overall performance met my expectations for a playable prototype.
10. The gameplay felt engaging and coherent.

**Participants**: Our target audience is mainly university students who enjoy narrative-driven puzzle games, stylish indie games, and exploration. We also include some causal players who play games less often, but enjoy aesthetics/emotion. To ensure the representativeness, we aim to balance game familiarity (70% who play games often, 30% otherwise) and gender (males/females).

**Stratified quota sampling**: 
- Cartoon-style/Indie game players: 4 (40%)
- Patient puzzle / exploration players: 4 (40%)
- Casual players: 2 (20%)

**Recruitment channel**: 
- University / campus participant pools — target students in arts/CS/games courses.
- Social media (Facebook/Instagram) with a short playtest call.
- Mutual friends who play games. 

**Data collection**:
- For think-aloud observation, we will collect qualitative (descriptive) data from observation and field notes.
- For post-task walkthroughs, we will collect qualitative (descriptive) data when showing transcripts/footage of what they did. 
- For questionnaires, we will collect quantitative data from ten written questions scaled from 1-5 to evaluate perceived usability, enjoyment, difficulty, immersion, and narrative understanding.  

**Data analysis**:
- For qualitative data, we will categorize all player quotes into aspects such as "Understanding goal", "Confusion/Misinterpretation", "Feedback Recognition" and "Emotional responses". This way helps us to analyse overall players' real-time reactions and know what aspect to be addressed. 
- For quantitative data, we will address following aspects: Logics clarity, Feedback Responsiveness, Narrative Engagement, Satisfaction / Flow. With scalar 1-5, we aim to get an average score of 4 in every aspect. 

**Timeline**: 
- We will conduct our evaluations with our improved (with animation & special effect) Demo game (from the 20th to the 27th of October) and complete the final game by Milestone 6. Report to be done by Milestone 6. 

**Responsibility**
- Patrick to improve game levels and overall developments based on player feedbacks. 
- Ryan to add the sound/music effect, models and finalise shaders. 
- Joly to add animations and special effect and conduct data analysis. 
- Every team needs to conduct game evaluation with 4 players and collect data.


## Evaluation Report

From think-aloud notes: 
- Most participants found the gameplay unclear when the LifeRoom started. Due to limitied interactions in the first stage, some users expreesed "**Confused**" feelings.
- After 1-2 minutes average when participants exited from the first door and entered another space (OutWorld), 7 users showed **excitement** and had a good understanding of the door/size transitions between the rooms.
- When the XS size player entered into the first room, half of the participants expressed "**interesting**" while the other half of participants found it "confusing".
- 
- During XS size player's exploration in the first room, 6 participants found the movement&control a bit tricky. Since only specific areas can trigger the movement, this technical issue caused some **frustrations**. And walking-stairs animation was still imcompelte, this didn't meet users' expections either.
- 



Here is a detailed summary of the questionnaire results and changes we have made to the game. 
| **Question* | **Gameplay Area Addressed** | **Average scores** | **Participants' Feedback** | **Changes** | 
|-------|-------------------|--------|----------------------------|-----------------------------|
| Q1 | Control & Interaction design | 3.0 | While the mouse control of player movement felt intuitive, it was hard to control since most evaluations were done on a Mac with TrackPad. | No changes to the game. Suggestion is to use a physical mouse to play. | 
| Q2 | Technical performance | 3.6 | There were two bugs tested out during evaluations when the player walks into hidden areas or accidentally into objects. Jumping and climbing animations were not completed. |   | 
| Q3 | Gameplay clarity | 3.5 | It was challanging for half of the users to understand how to progress the game, partially because they didn't watch trailer as well. Some users think it's acceptable that a puzzle game takes a bit longer than usual games to figure out the goal/progression. | A UI intro page was added at the start of the game for more clarity. | 
| Q4 | Narrative communication | 3.5 | The background transition was not complete. | Background change and UI for story dialogue |
| Q5 | Level design | 4.0 |  The bedroom and workplace layout look pretty and natural. |  |
| Q6 | Narrative communication | 3.5 | Without watching trailer or brief introduction of the game background, it's hard to know meaning of rooms. | UI for story dialogue | 
| Q7 | Visual design | 4.0 | Visual elements look decent yet simple. Limited to three small spaces. |    | 
| Q8 | Audio design | 4.0 | The music feels natural and comfortable. Not too relevant with player itself.  |       | 
| Q9 | Overall experience | 3.9 | It's a playable game however can be very hard to play. | Created some glow and outline effects for hints. | 
| Q10 | Engagement | 4.0 | At this stage, it's lack of excitement and interactions with different objects. With more developed elements, it'd be an interesting game to play. |     | 

## Shaders and Special Effects

TODO - see specification for details

## Summary of Contributions

TODO - see specification for details

## References and External Resources

TODO - see specification for details




















