This is my daily study log that will contain all my notes and logs of the working day. This will have about 520 logs, since that equates to 2 years of working on weekdays!

### FND-01

**27-07-2026:**

I learned how to create an issue template, how to set up a nice looking readme, and how to work with markup in text files to make the layout nicer.

### FND-02

**28-07-2026:**

I learned how to retrieve an element in an enum from its set value (eg. ElementName = 1. Where 1 is the value). I also learned how to check each individual char of a string with the .Any extension function to then check if the string contains any numbers.

**29-07-2026:**

I learned that when returning a static list (in a static class), it returns a reference to that list instead of a copy.

**30-07-2026:**

Today I didn’t necesarilly learn much, but I did make a bunch of progress in my 2 hours of working time. I’m nearing the end of this project, exactly as planned on schedule!

**31-07-2026:**

I learned that when you want to create/write markdown files in C\#, that you can’t just do that within a normal string and write that to a file with the .md extension and syntax. Instead, you need to use an external library (in my case I used Aspose) to be able to configure and create markdown files, which honestly seems very overcomplicated in my opinion, C\# should have the functionality built-in natively. I made nice progress on the project and am near my deadline. My next (and last) session of this project will be polishing everything, mostly the markdown file. The main program is basically fully done and functioning!

**03-08-2026:**

I finished the project with 2 things I was unable to resolve in time. I was unable to implement the markdown file as intended, which I thought was only possible with an external library, but apperantly you can also create markdown files with the StringBuilder class. Secondly I was unable to resolve the bug which shows the index of the todo’s importancy instead of its name.

### WEB-01

**04-08-2026:**

I started working on my webpage portfolio project assignment. I actually find it pretty fun and satisfying to create a webpage of mine! I learned how to use various tags with the help of the official W3Schools.com docs. I found out that, at least creating fairly simple webpages, is pretty simple and easy to do. I will definitely try to make something nice out of this to showcase my portfolio properly! I didn’t encounter problems that made me get stuck yet, so nothing to report.

**05-08-2026:**

I’ve added a lot of text to fill up the website more as preperation for the implementation of the required navigation anchoring. I’m not done yet, since it still isn’t yet scrollable (at least not in fullscreen), but it’s getting close. I’ve learned how to implement tables and got a significantly deeper understanding in how styling works, especially styling elements, like tables, etc. It’s really starting to come clear that HTML is not the thing that’s hard about programming a website, it is actually thinking about how you even want it to look like in the first place and then try to find a way on how to implement exactly that, which, most of the time, requires extensive searching to figure out what element you’re looking for that does that. I did get a little stuck today. I was trying to figure out how to properly implement some layouts, that seperate each section appropriately, I haven’t yet figured out how to do this the way I want to, thus at the top of the site there is an attempt at a ‘top navigation bar’ which resulted in having useless buttons sitting at the top of the page. I definitely need to deepen my research a bit on how to implement verticale and horizontal layouts. I also had a bit of trouble getting the text formatted correctly, for now I’ve put the headers outside of their segments to mostly resolve this issue. I’ve come to a conclusion that this webpage won’t only be for my portfolio, but instead basically everything, basically my personal archive that includes blog posts that I can read in the future to refresh my memory on a certain topic. I think this will be a very worthwhile investment!

**06-08-2026:** abscent

**07-08-2026:**

I was able to add a (functioning) menu bar on the top of the website, though I wasn’t able to make it horizontal for some reason. I’m still very much struggling with layouts, I just can’t figure it out, having led to frustration. I currently want the portfolio section to have a left and right side, with the left side displaying the projects tabel, and the right side some text.

**10-08-2026:**

I was finally able to find out on how to make the portfolio section have a section on the left, and one on the right. I’m nearly done with this assignment, and the only thing currently holding me back is properly implementing the image (logo). I’ve relocated the image to be at the bottom of the page since that is in my opinion a more fitting position, but when it is put there, the ‘sources’ and footer section(s) get pushed down because of the image ‘collider’ box. I need to find a way on how to fix this issue, and essentially make the two bottom sections ignore the box of the image.

**11-08-2026:**

I learned that you can controll the closeness of the next element by adjusting the (bottom) margin of the element above it, which basically acts like a collider. Despite this discovery, I didn’t manage to get the “Sources” section to be pushed up. I don’t bother, because it still looks good the way it looks now. I also wasn’t able to get the top navigation bar to be horizontal, but again, I find it okay the way it is right now. I’ve finished this assignment now and will be moving onto the next one!

### WEB-02

**12-08-2026:**

I’ve started working on this new assignment, which requires me to recreate a basic public website layout. I’ve chosen to do a nice news-article-like card grid. I’ve learned a lot during this first session, and it’s all logged inside the file (for the specific commit on this date). I’ve encountered a bit of a confusion, but it’s too imature to 100% I’m stuck, so I will be looking further into this tommorow, it’s basically me not getting the text of the card to be within the actual card’s (visible) area. There’s also a problem with the scalling of the cards, mostly making them too small for my liking. Today was a very valuable day!

**13-08-2026:**

I made a decent amount of progress, and am essentially already almost done, there’s just this critical bug I’m encountering that makes the text that’s supposed to be in the card be outside of it, on top. I have no idea how to fix this, so it’s going to be a hard time. I did learn a decent amount once again, which is all mentioned in the file like always, examples are: knowing how to round the corners of an element, how to center the contents of a parent, etc.

**14-08-2026:**

I’ve learned that you can literally style the html itself. Also that you can use the \* symbol in the styling area to style all the elements in the entire html. I did use AI because I got stuck with trying to get the text within the cards, and turns out that it had all to do with the relative and absolute positioning of the elements. I’m not exactly sure how to do it yet, because I didn’t ask AI on HOW to do it, but rather what the problem was, which was the positioning. I’ve learned the difference about the relative and absolute position attributes for an element and that an element with an absolute positioning NEEDS to have a parent of some sort that has a relative position (otherwise it will anchor to the most nearby relative position element).

**17-08-2026:**

I learned a lot about scalling elements, especially on how to implement “breakpoints” to scale the screen accordingly toward the device screen size. In my project, I’ve made it so that when you’re on a mobile device, the cards will switch to a verticle grid where they are stacked onto eachother instead of pairs of two being next to eachother. I’m so close on finishing this project. The only thing that’s left to do is add some sort of “focus” indicator when clicking/selecting a card, which doesn’t work yet since the “:hover” function is interfering with that.

**18-08-2026:**

Today I finished the project. I was struggling A LOT with implementing focus correctly, because of that I unfortunately had to use the help of an AI. The problem was that when clicking the card, the \<a\> element instead would receive the focus instead of the card. The fix was to use “:focus-within” which also registeres when children in the parent get focused.

### WEB-03

**19-08-2026:**

Started working on the project immediately learning on how to use input elements. I’m having a hard time figuring out on how to retrieve the values of those input fields though.

**20-08-2026:**

I learned on how to create “nodes” (elements) in javascript and how to add those to parents as children (well… not really because it didn’t work as expected, but still). I also became a little more comfortable with classes and functions, so that’s excellent!

**21-08-2026:** abscent

**23-08-2026 – 26-08-2026:** away from home

**27-08-2026:**

I didn’t learn too much this session. I tried troubleshooting on how to make the script not complain that the element (the container that holds all the session elements) is null on start, because the script is being run before the DOM has finished intializing. I used an even listener that should’ve fixed the issue, and I think it has, but I’m now getting a new error that isn’t clear as to what the problem is. Next session I think I’ll work on starting a timer and forget about showing the sessions to the user for a bit.

**28-08-2026:**

I was kinda confused this session with everything. There were a bunch of errors and I didn’t really fully get the implementation of the timer. I managed to find a nice article of someone that explained how to implement a timer that you can view in the UI. I think I get the system now, but it was a bit confusing at first. Might be his implementation that’s just confusing to me, or maybe it’s just really me. Either way, the timer works (mostly, with 1 bug I found).

**31-08-2026:**

During this session I've become even more comfortable with styling, which is excellent! I also learned how to change the visiblity of elements, especially in javascript. This session was mostly just fixing and adding stuff, so I didn't necesarilly learn much, but I was pretty productive!

**01-09-2026:**

This session was a bit disasterous because of me not being able to figure out what was causing a specific error/bug with the timer. Luckily, I found the problem at the last moment. To compesate for all the lost time, I quickly implemented a "clear all session" button that learned me how to remove a/all child(ren) from a parent. I think I'm nearly done with this project. I will be doing some more polishing tomorrow in terms of UI, but it should be almost done.

**02-09-2026:**

I'm nearly done finishing the project. The only thing I still need to do is add an 'empty' state for the sessions when no sessions are added yet. I've not done this today yet, because I'm also required to write article/blog post, which takes a lot of time. And because of the rough new schedule I now have to follow because of school, I'll just leave those
two things for tommorow. I did fix a pretty interesting bug, which is listed in the 'what I've learned this session' part at the bottom of the index.html file

**03-09-2026:**

I've finished the project. Added the 'No Sessions' label when there aren't any created sessions, which represents the required empty state.

### PY-01

**04-09-2026:**

I started working on this new assignment. This is honestly way to easy, but it did refresh my memory A LOT in terms of syntax, so it's definitely important I do this still.
I've made the command lines a little harder and more complex to at least make it a bit interesting, but it's still extremely easy 
(since what I'm doing isn't that complex, just some basic account system). I tried finishing the project entirely today, but loading with json got me stuck, because apperantly
json doesn't like it when you try to save a dictionary with a tuple as key to json.

**05-09-2026:**

Opened the project, but was too demotivated to do anything. I did figure out one thing, and that is what I should be doing the next session, which is find a way to convert the current date I want to save to a saveable and loadable json format, since the format I'm currently perusiading isn't compatible with the working flow of json. This is a reletively intensive task, because I'm not THAT familiar with how json works, so that's why I won't be doing this today. I also added a bit more projects I've worked on before, since it is my full portfolio after all. Though I noticed that the file sizes are getting a bit big, so I might stop saving the entire projects and only include the actual scripts.

**06-09-2026:**

I finished the account project by implementing the save and load system properly. It was honestly extremely easy to do so, I just had to think a little about how I wanted to do it. I went for the architecture to just make a __dict__ out of all the UserInformation classes that were saved in the dictionary and add those to a list. Then save that list to a json which I then can load and just create new UserInformation classes from the values of the __dict__ within the list. I also started a new project, since I hadn't yet used the required sets. I made a number guessing game, which was first that came to mind after reading what sets are for and how they work. I finished that project today as well.

**07-09-2026 - 15-09-2026:**

Descision made that I will no longer be doing programming every day, far from that even. Since the new school year has started, there's just no time and discipline/motivation left in me after the long school days I need to make + all of the homework I need to do. This does not mean I'm completely stopping with this, but I will be significantly slowing down stuff, and thus ignoring the given dead lines on the assignment(s).

**19-09-2026:**

I've found out that working in the weekends is basically my only choice from now on, so that's what I'll be doing. I'll try to throw in slightly longer sessions as well.
I'm pretty confused currently with the csv.DictWriter, because apperantly you can't assign any value without it throwing an error after accessing it when the DictWriter stream
is closed. So apperantly it isn't a copy of the value of the DictWriter, but instead a reference? Anyway, my code is a little spaghetti momentarily, so I'll have to refactor it 
a bit tommorrow. And I'll also have to find a way to resolve this problem with the DictWriter.

**26-09-2026:**

It's getting pretty bad; no motivation at all. I described briefely what I did in the 'What I've learned this session' section in the code file. So if interested, check that out.

**02-10-2026:**

It was a successful day, I was able to finish the project by implementing the remaining summaries, and command line commands! I hope I can get decent progress on the new project
tomorrow and the day after!

<<<<<<< Updated upstream
=======
### WEB-04

>>>>>>> Stashed changes
**03-10-2026:**

I didn't do any programming, instead I made the required brief for the website I will be creating. I have a pretty complex/big idea for it, so I might extend the deadline quite a
bit since I think I can learn a lot from this. The briefing has all the required stuff in it and includes a detailed description and layout of the website.

**06-10-2026:**

One school lesson got cancelled, so I got home earlier, thus I had time  to actually do programming, yay! I'm actually genuinely impressed by myself in terms of what I was able to do today and I think I did the styling pretty nicely, eventhough it's all just drafts right now. I didn't learn much, but I was able to refresh my memory on how HTML works and all, so that's nice.

**10-10-2026:**

I started making the top hamburger navigation menu bar, with the help from W3schools, since I'm physically unable to do this myself (as already explained in the 'what I've learned this session within the index.html file). I sure do hope I get better with this type of stuff, but I don't know. Anyway, I also created a fast prototype for the community page. I'm honestly not really sure as to how far I want to go with this webpage in terms of polishing and features. It's probably wise to not go crazy, since I already know creating webpages will definitely not be my special department later.

**14-10-2026:**

I've starting with actually properly reading documentation on how everything is supposed to work, since I'll need a deep understanding of how everything works before being able to make it correctly myself. I learned about how relative/absolute positioning can affect how children behave on scalling of the parent. I'll definitely be spending a lot more time actually reading stuff instead of only doing stuff, which, to me always feels like I'm doing nothing, but that's not true.

**15-10-2026:**

I learned a lot about how to position and scale things the way I want. I also learned how to make a nice layout with a grid display like here: https://www.w3schools.com/css/tryit.asp?filename=trycss_grid_layout_named, which is with an interesting property named "grid-template-areas". I learned how to position a flex item to the right side of a flexbox display with the margin-left being set to "auto". Basically, I got more comfortable with the positioning and understanding of items within different displays!

**16-10-2026:**

S-tier session. It was an absolute vibe. I'm actually really liking web development all of a suddent since the last couple of days! I guess the beginning just sucks because you aren't comfortable with all the styling and stuff. Now, I keep learning new stuff every day and getting more comfortable with the stuff I already know. This webpage is genuinely starting to look pretty decent. Sure, not as good as most modern sites do nowadays, but for a non-artist dev, it's pretty nice imo. I'm really noticing I keep improving and getting more comfortable with all the stuff I need to do. I might actually create a really nice portolio site for myself to show off my portfolio, which... technically I already started making with my first ever web assignment, but it's probably best to start over, since my knowledge has greatly improved. I love to learn new stuff and eventually get comfortable and good at it, it's genuinely awesome! The next big subject I'll tackle soon will probably be cybersecurity. I think I'm going to try to hack my own school's site, since it looks extremely unpolished (worse than this site, like... WAY worse, with blank pages all over the place). Of course, if I actually manage to hack the site, I won't be doing any evil stuff or something like that. But it's just I perfect site to hack I think.

**17-10-2026:**

This was a pretty mixed session, with me working on a lot of different things, mostly fixing and polsishing (by adding new functionalities to elements). Tomorrow I'll finally try to implement the actual google map thingy that shows a real location, should be interesting. There's definitely still a lot to learn about javascript, I just now discovered javascript has some sort of querry system with being able to make jQuerry objects. I'd say my effeciency wasn't great, though not bad either. I worked for a fat 2 hours straight, the time flew by, which is also partly why I feel like I didn't do as much as I could've done. Anway, everything worked out pretty well so there's that.

**19-10-2026:**

Pretty bad session, ain't gonna lie. I've managed to implement the google maps widget thingy, which was way easier than it should've been. That's basically about it. I started implementing logic for checking the input in the forum area (join us), but my typing efficiency has just vanished into the void. It's so bad, I really need to fix my typing accuracy, because it's getting really bad. I guess it's because I don't spend enough time programming anymore because of my annoying school times, ugh. So yeah, I also added some nice labels that show the user when a field is invalid, but those don't yet work the way I want them to work. When I set the display to be inline, the reset just gets thrown all over the place for some reason, so I'll have to look into that. Anyway, I'm hoping on a better, more efficient session tomorrow.
Pretty bad session, ain't gonna lie. I've managed to implement the google maps widget thingy, which was way easier than it should've been. That's basically about it. I started implementing logic for checking the input in the forum area (join us), but my typing efficiency has just vanished into the void. It's so bad, I really need to fix my typing accuracy, because it's getting really bad. I guess it's because I don't spend enough time programming anymore because of my annoying school times, ugh. So yeah, I also added some nice labels that show the user when a field is invalid, but those don't yet work the way I want them to work. When I set the display to be inline, the reset just gets thrown all over the place for some reason, so I'll have to look into that. Anyway, I'm hoping on a better, more efficient session tomorrow.

**20-10-2026:**

I fucked up pretty bad with my git repo, but I think most is fine now. Apperantly I had a mergin conflict and somehow history was rewritten in a way so that it was way behind the actual point it should be at. Luckily I have this branch separate, so I was able to just get my stuff back, otherwise this whole project would've been gone. Anyway, I managed to add the logic I wanted to the join us page, but I think I'm leaving it as is, since sending an actual approvable email request basically requires online servers to be up, which this site is obviously not run on any server (well.. technically it is on my local PC). I'm not sure how I want to continue with that, so I moved on to creating the 'create article' section where users can post articles. Normally, the plan was to have only joined users be able to post, but that'd mean I'd have to implement a whole account system with data base and all, which I'm fine with since I'd learn a lot more about javascript, but it won't really lead anywhere because proper submissions can't really be implemented. I'll maybe do some more research later to see if it maybe is possible to this all locally and thhink about if it's still worth it anyway to implement the profiles/accounts.

**21-10-2026:**

Neutral session. I learned on how you can pass in parameters to function via the "addEventListener" function by just adding an anonymous function witin it. I also refined some of my event handling skills and refactored the code to be a little cleaner, which means I'm getting a little more comfortable with javascript, which is exactly what I wanted! Next session I'll finish the page and I think I'll begin on implementing profiles, since I've decided I want to fidget with databases in javascript to get to know javascript better.

**22-10-2026:**

Pretty nice, relaxing session. I learned a lot by getting some short advice from chatGPT. Because of chatGPT, I got to learn what the vh/vw and fr units stand for, which are actually really helpful! I made a nice layout for the 'create article' page, which was what I needed help with from chatGPT. I needed to make a small change to make it work as I wanted, so I was genuinely very close by just doing it all by myself (without documentation from others online), meanning I'm genuinely getting better, still. I also learned how to open a window via javascript (which I usually do with a <a> element)

**23-10-2026:**

Unfortunately I was a bit short on time, but I did manage to make a small start on my new, occasional subject exploration of cyber security, hacking. This is an area I've wanted to look into for quite a while now to see if I like it and to just understand the concept of how it generally works for basic knowledge. I haven't learned much since I basically only was able to gather resources that are actually useful to read through. One of those resources is a website that literally lets you practise this stuff and learn about it (name is hack the box). I've also saved a couple of pages about subjects I should understand more deeply, like how pings work, VPNs work, etc. I'll be working on this tomorrow as well. Then next week, I'll continue with the WEB-04 assignment.

**24-10-2026:**

I started the junior beginner path in hack the box. I'm really liking it, eventhough I've only done 2 sections. I'm taking it slowly since I'm also making my own notes in word to review later. It takes a long time to write stuff over in my own words, which is why it took me about 2,5 hours to do the first two sections.

**27-10-2026:**

I managed to do something on a normal school day. Who knows, maybe I just had to get used to this new routine. I'll try to do as much programming as possible during weekdays. Since I want to do the cybersecurity course stuff in the weekend (since I need 2-3 hours for that), so if I can work on my assignments during most weekdays, I won't be missing anything. Though I don't think I'll be able to do this consistently, because I usually need the time to study for upcoming tests (which I finished earlier with today since I had time at school to do so). I've decided I'm going to run my own local server on my PC to then communicate with this webpage. To save articles and user information, meaninig I've also decided to escalate this project a bit further into more complex ground (handling databases and encryption for user profiles.). I'll probably need another 10 hours to do all that, so I'll be working about 20 hours on this project in total, which is not too much over my deadline. The upcoming projects/assignments do not look easy, so I'll have to really start locking in on stuff, I guess the AI who created these assignments thought it was clever to start off very easy, then out of nowhere throw insane projects at me. Anyway, that's exactly what I want, so I won't complain.