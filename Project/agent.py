import argparse
import sys
import pdb
import numpy
import scipy
import gymnasium as gym
from gymnasium import wrappers, logger
import matplotlib.pyplot as plt
import json

#Author: Lily Ready lr4631

#Helper functions because I keep repeating myself
#Randomized movement functions
#Use this if the block is in the middle, all moves available
def middle_move():
    choice = numpy.random.randint(0,4)
    #Down left
    if choice == 1:
        return 5
    #Up right
    if choice == 2:
        return 2
    #Up left
    if choice == 3:
        return 4
    #Down right
    return 3
#Used if the block is the leftmost block
def left_move():
    choice = numpy.random.randint(0,3)
    if choice == 1:
        return 5
    if choice == 2:
        return 2
    return 3
#Used if the block is the rightmost block
def right_move():
    choice = numpy.random.randint(0,3)
    if choice == 1:
        return 5
    if choice == 2:
        return 4
    return 3

#Locates springy and returns x & y coordinates
def find_springy(observation):
    x,y,z = numpy.where(observation == (146,70,192))
    return x,y
#Reset graph to default values
def reset_graph(graph):
    graph[0][5] = 1
    graph[1][4] = 1
    graph[1][6] = 1
    graph[2][3] = 1
    graph[2][5] = 1
    graph[2][7] = 1
    graph[3][2] = 1
    graph[3][4] = 1
    graph[3][6] = 1
    graph[3][8] = 1
    graph[4][1] = 1
    graph[4][3] = 1
    graph[4][5] = 1
    graph[4][7] = 1
    graph[4][9] = 1
    graph[5][0] = 1
    graph[5][2] = 1
    graph[5][4] = 1
    graph[5][6] = 1
    graph[5][8] = 1
    graph[5][10] = 1
    return graph

#Updates the graph so we know where to go
def update_graph(observation,score_color,graph):
    #If cube is the same color as score, that cube is worth 1
    #If springy is on that cube, treat it like it's not connected
    #If cube is a color other than coil or score, then cube is worth 10
    if numpy.array_equal(score_color,[0,0,0]):
        return graph
    #Row 1
    #Reset, then set changes
    graph = reset_graph(graph)
    if numpy.where(observation[35][77] == score_color)[0].size:
        graph[0][5] = 10
    #Row 2
    if numpy.where(observation[65][65] == score_color)[0].size:
        graph[1][4] = 10
    if numpy.where(observation[65][95] == score_color)[0].size:
        graph[1][6] = 10
    #Row 3
    if numpy.where(observation[95][55] == score_color)[0].size:
        graph[2][3] = 10
    if numpy.where(observation[95][77] == score_color)[0].size:
        graph[2][5] = 10
    if numpy.where(observation[95][105] == score_color)[0].size:
        graph[2][7] = 10
    #Row 4
    if numpy.where(observation[121][45] == score_color)[0].size:
        graph[3][2] = 10
    if numpy.where(observation[121][65] == score_color)[0].size:
        graph[3][4] = 10
    if numpy.where(observation[121][95] == score_color)[0].size:
        graph[3][6] = 10
    if numpy.where(observation[121][115] == score_color)[0].size:
        graph[3][8] = 10
    #Row 5
    if numpy.where(observation[150][30] == score_color)[0].size:
        graph[4][1] = 10
    if numpy.where(observation[150][55] == score_color)[0].size:
        graph[4][3] = 10
    if numpy.where(observation[150][77] == score_color)[0].size:
        graph[4][5] = 10
    if numpy.where(observation[150][105] == score_color)[0].size:
        graph[4][7] = 10
    if numpy.where(observation[150][130] == score_color)[0].size:
        graph[4][9] = 10
    #Row 6
    if numpy.where(observation[181][15] == score_color)[0].size:
        graph[5][0] = 10
    if numpy.where(observation[181][45] == score_color)[0].size:
        graph[5][2] = 10
    if numpy.where(observation[181][65] == score_color)[0].size:
        graph[5][4] = 10
    if numpy.where(observation[181][95] == score_color)[0].size:
        graph[5][6] = 10
    if numpy.where(observation[181][115] == score_color)[0].size:
        graph[5][8] = 10
    if numpy.where(observation[181][140] == score_color)[0].size:
        graph[5][10] = 10
    #Check spring
    s,p = find_springy(observation)
    #If no spring leave, otherwise find and mark the box
    if not s.size:
        return graph
    #Super cautious, don't move anywhere springy can move to
    if s[0] in range(20,30) and p[0] in range(60,80):
        graph[0][5] = 9999
        graph[1][4] = 9999
        graph[1][6] = 9999
    #Row 2
    elif s[0] in range(40,60) and p[0] in range(60,70):
        graph[0][5] = 9999 
        graph[1][4] = 9999
        graph[2][3] = 9999 
        graph[2][5] = 9999 
    elif s[0] in range(40,60) and p[0] in range(90,100):
        graph[0][5] = 9999
        graph[1][6] = 9999
        graph[2][5] = 9999 
        graph[2][7] = 9999 
    #Row 3    
    elif s[0] in range(68,87) and p[0] in range(40,60):  
        graph[1][4] = 9999
        graph[2][3] = 9999 
        graph[3][2] = 9999
        graph[3][4] = 9999
    elif s[0] in range(68,87) and p[0] in range(60,80):  
        graph[1][4] = 9999
        graph[1][6] = 9999
        graph[2][5] = 9999  
        graph[3][4] = 9999
        graph[3][6] = 9999
    elif s[0] in range(68,87) and p[0] in range(80,100): 
        graph[1][6] = 9999
        graph[2][7] = 9999   
        graph[3][6] = 9999 
        graph[3][8] = 9999
    #Row 4
    elif s[0] in range(95,115) and p[0] in range(40,60):  
        graph[2][3] = 9999 
        graph[3][2] = 9999
        graph[4][1] = 9999
        graph[4][3] = 9999
    elif s[0] in range(95,115) and p[0] in range(60,80):  
        graph[2][3] = 9999
        graph[2][5] = 9999
        graph[3][4] = 9999
        graph[4][3] = 9999
        graph[4][5] = 9999
    elif s[0] in range(95,115) and p[0] in range(80,100):
        graph[2][5] = 9999
        graph[2][7] = 9999  
        graph[3][6] = 9999
        graph[4][5] = 9999
        graph[4][7] = 9999
    elif s[0] in range(95,115) and p[0] in range(100,120):
        graph[2][7] = 9999 
        graph[3][8] = 9999
        graph[4][7] = 9999
        graph[4][9] = 9999    
    #Row 5
    elif s[0] in range(125,145) and p[0] in range(30,50):
        graph[3][2] = 9999
        graph[4][1] = 9999
        graph[5][0] = 9999
        graph[5][2] = 9999
    elif s[0] in range(125,145) and p[0] in range(50,70):
        graph[3][2] = 9999
        graph[3][4] = 9999  
        graph[4][3] = 9999
        graph[5][2] = 9999
        graph[5][4] = 9999
    elif s[0] in range(125,145) and p[0] in range(70,90):  
        graph[3][4] = 9999
        graph[3][6] = 9999
        graph[4][5] = 9999
        graph[5][4] = 9999
        graph[5][6] = 9999
    elif s[0] in range(125,145) and p[0] in range(90,110):
        graph[3][6] = 9999
        graph[3][8] = 9999
        graph[4][7] = 9999  
        graph[5][6] = 9999
        graph[5][8] = 9999  
    elif s[0] in range(125,145) and p[0] in range(110,130):
        graph[3][8] = 9999
        graph[4][9] = 9999    
        graph[5][8] = 9999
        graph[5][10] = 9999
    #Row 6
    elif s[0] in range(155,175) and p[0] in range(20,40):
        graph[4][1] = 9999 
        graph[5][0] = 9999
    elif s[0] in range(155,175) and p[0] in range(40,60): 
        graph[4][1] = 9999
        graph[4][3] = 9999
        graph[5][2] = 9999
    elif s[0] in range(155,175) and p[0] in range(60,80): 
        graph[4][3] = 9999
        graph[4][5] = 9999
        graph[5][4] = 9999
    elif s[0] in range(155,175) and p[0] in range(80,100): 
        graph[4][5] = 9999
        graph[4][7] = 9999
        graph[5][6] = 9999
    elif s[0] in range(155,175) and p[0] in range(100,120): 
        graph[4][7] = 9999
        graph[4][9] = 9999
        graph[5][8] = 9999
    elif s[0] in range(155,175) and p[0] in range(120,140): 
        graph[4][9] = 9999
        graph[5][10] = 9999
    return graph

#Uses a modified BFS to find the closest uncolored square
def find_closest(graph,x,y):
    queue =[]
    queue.append([[x,y,0]])
    seen = [(x,y)]
    escapeCont = 40
    while len(queue) and escapeCont:
        path = numpy.array(queue.pop(0))
        end = path[-1]
        escapeCont -= 1
        for x2, y2, dir in (end[0]-1,end[1]+1,2),(end[0]+1,end[1]+1,3),(end[0]-1,end[1]-1,4),(end[0]+1,end[1]-1,5):
            if(0<= x2 < 6) and (0 <= y2 < 11) and (graph[x2][y2] != 9999) and not numpy.where(seen == (x2,y2))[0].size:
                newpath = list(path)
                newpath.append([x2,y2,dir])
                queue.append(newpath)
                seen.append((x2,y2))
        if end[0] == x and end[1] == y:
            continue
        if numpy.shape(path)[0] != 1 and graph[end[0]][end[1]] == 1:
            return path[1][2]
    return 0

class Agent(object):
    left_hover = 1
    rightHover = 1
    graph = numpy.array(
        [[9999,9999,9999,9999,9999,1,9999,9999,9999,9999,9999],
         [9999,9999,9999,9999,1,9999,1,9999,9999,9999,9999],
         [9999,9999,9999,1,9999,1,9999,1,9999,9999,9999],
         [9999,9999,1,9999,1,9999,1,9999,1,9999,9999],
         [9999,1,9999,1,9999,1,9999,1,9999,1,9999],
         [1,9999,1,9999,1,9999,1,9999,1,9999,1],])
    score_color = (0,0,0)
    base_color = (0,0,0)
    """The world's simplesst agent!"""
    def __init__(self, action_space):
        self.action_space = action_space

    # You should modify this function
    def act(self, observation, reward, done):
        # Qbert is 181, 83, 40
        # Snake is 146, 70, 192
        # Create graph to map move options
        # Weight against coil paths heavily
        # Weight cubes not matching score lightly
        # Check few pixels above center of cube to see if qbert or snake is there
        # Score is same color as goal 210,210,64 for first level
        # Score is located at observation[006][35-71], check here to log color
        if(self.score_color == 0,0,0) and numpy.where(observation[6][36] != (0,0,0))[0].size:
            self.score_color = observation[6][36]
        
        #When starting a new level, reset everything
        col = numpy.where(observation[180][12] != (0,0,0))[0]
        if(self.base_color == 0,0,0) and col.size:
            col = observation[180][12]
            if numpy.where(self.base_color != col)[0].size and numpy.where(col != self.score_color)[0].size:
                self.base_color = observation[180][12]
                self.graph = reset_graph(self.graph)
                self.score_color = 0,0,0
        #Find  Qbert
        #top of pyramid = (center)17-36  76,77,78,79
        x,y = (numpy.where(observation[30] == (181,83,40)))
        if x.size and x[0] in range(74,81):
           #Update the board before qbert moves
            self.graph = update_graph(observation,self.score_color,self.graph)
            dir = find_closest(self.graph,0,5)
            if dir:
                return dir
            choice = numpy.random.randint(0,2)
            if(choice):
                return 5
            return 3
        #row 2 
        elif numpy.where(observation[64] == (181,83,40))[0].size:
            self.graph = update_graph(observation,self.score_color,self.graph)
            x,y = (numpy.where(observation[64] == (181,83,40)))
            #Right
            if x[0] in range(90,97):
                dir = find_closest(self.graph,1,6)
                if dir:
                    return dir
                return right_move()
            #Left
            elif x[0] in range(62,69):
                dir = find_closest(self.graph,1,4)
                if dir:
                    return dir
                return left_move()
        #row 3
        elif numpy.where(observation[92] == (181,83,40))[0].size:
            self.graph = update_graph(observation,self.score_color,self.graph)
            x,y = (numpy.where(observation[92] == (181,83,40)))
            #Right
            if x[0] in range(102,109):
                dir = find_closest(self.graph,2,7)
                if dir:
                    return dir
                return right_move()
            #Left
            elif x[0] in range(50,57):
                dir = find_closest(self.graph,2,3)
                if dir:
                    return dir
                return left_move()
            #Middle
            elif x[0] in range(72,79):
                dir = find_closest(self.graph,2,5)
                if dir:
                    return dir
                return middle_move()
        #Row 4
        elif numpy.where(observation[121] == (181,83,40))[0].size:
            self.graph = update_graph(observation,self.score_color,self.graph)
            x,y = (numpy.where(observation[121] == (181,83,40)))
            #Right
            if x[0] in range(116,120):
                dir = find_closest(self.graph,3,8)
                if dir:
                    return dir
                return right_move()
            #Left
            elif x[0] in range(40,45):
                dir = find_closest(self.graph,3,2)
                if dir:
                    return dir
                return left_move()
            #Middle
            elif x[0] in range(91,94):
                dir = find_closest(self.graph,3,6)
                if dir:
                    return dir
                return middle_move()
            elif x[0] in range(62,69):
                dir = find_closest(self.graph,3,4)
                if dir:
                    return dir
                return middle_move()
        #Row 5
        elif numpy.where(observation[150] == (181,83,40))[0].size:
            self.graph = update_graph(observation,self.score_color,self.graph)
            
            #Find springy and update hovers
            s,p = find_springy(observation)
            if numpy.where(observation[138][15] != (0,0,0))[0].size:
                self.left_hover = 1
            if numpy.where(observation[138][145] != (0,0,0))[0].size:
                self.rightHover = 1
            #Then do the normal stuff
            x,y = (numpy.where(observation[150] == (181,83,40)))
            #Right
            if x[0] in range(124,133):
                #Prioritize killing springy over getting points
                if self.rightHover and p.size and p[0] in range(100, 150):
                    self.rightHover = 0
                    return 2
                dir = find_closest(self.graph,4,9)
                if dir:
                    return dir
                return right_move()
            #Left
            elif x[0] in range(26,35):
                #Prioritize killing springy over getting points
                if self.left_hover and p.size and p[0] in range(10, 50):
                    self.left_hover = 0
                    return 4
                dir = find_closest(self.graph,4,1)
                if dir:
                    return dir
                return left_move()
            #Middle
            elif x[0] in range(102,109):
                dir = find_closest(self.graph,4,7)
                if dir:
                    return dir
                return middle_move()
            elif x[0] in range(50,57):
                dir = find_closest(self.graph,4,3)
                if dir:
                    return dir
                return middle_move()
            elif x[0] in range(72,79):
                dir = find_closest(self.graph,4,5)
                if dir:
                    return dir
                return middle_move()
        #Row 6
        elif numpy.where(observation[180] == (181,83,40))[0].size:
            self.graph = update_graph(observation,self.score_color,self.graph)
            x,y = (numpy.where(observation[180] == (181,83,40)))
            #Right
            if x[0] in range(136,143):
                return 4
            #Left
            elif x[0] in range(13,20):
                return 2
            #Middle
            elif x[0] in range(38,45):
                dir = find_closest(self.graph,5,2)
                if dir:
                    return dir
                choice = numpy.random.randint(0,1)
                if choice == 1:
                    return 4
                return 2
            elif x[0] in range(62,69):
                dir = find_closest(self.graph,5,4)
                if dir:
                    return dir
                choice = numpy.random.randint(0,1)
                if choice == 1:
                    return 4
                return 2
            elif x[0] in range(90,97):
                dir = find_closest(self.graph,5,6)
                if dir:
                    return dir
                choice = numpy.random.randint(0,1)
                if choice == 1:
                    return 4
                return 2
            elif x[0] in range(114,121):
                dir = find_closest(self.graph,5,8)
                if dir:
                    return dir
                choice = numpy.random.randint(0,1)
                if choice == 1:
                    return 4
                return 2
        #If No Qbert return nothing
        return 0

#Author Christopher Homan

## YOU MAY NOT MODIFY ANYTHING BELOW THIS LINE OR USE
## ANOTHER MAIN PROGRAM
if __name__ == '__main__':

    dict = {}
    values=[]
    for i in range(200):
        
        actionPath = []
        parser = argparse.ArgumentParser(description=None)
        parser.add_argument('--env_id', nargs='?', default='Qbert', help='Select the environment to run')
        args = parser.parse_args()

        # You can set the level to logger.DEBUG or logger.WARN if you
        # want to change the amount of output.
        logger.set_level(logger.INFO)

        env = gym.make(args.env_id)#, render_mode="human")
        state, info = env.reset()
        # You provide the directory to write to (can be an existing
        # directory, including one with existing data -- all monitor files
        # will be namespaced). You can also dump to a tempdir if you'd
        # like: tempfile.mkdtemp().
        outdir = 'random-agent-results'


        env.unwrapped.seed(0)
        agent = Agent(env.action_space)

        episode_count = 100
        reward = 0
        terminated = False
        score = 0
        special_data = {}
        special_data['ale.lives'] = 3
        observation = env.reset()[0]
        step = 0
        
        last_life = info.get("lives")
        while not terminated:
            
            action = agent.act(observation, reward, terminated)
            actionPath.append(int(action))
            #pdb.set_trace()
            observation, reward, terminated, truncated, info = env.step(action)
            if last_life != info.get("lives"):
                score -= (4 - info.get("lives")) * 50
                last_life = info.get("lives")
            score += reward
            #env.render()
        
        if(len(values) < 10):
            values.append(score);
            dict[len(dict)] = actionPath;
        else:
            s = min(values)
            if score > s:
                dict[values.index(s)] = actionPath
                values[values.index(s)] = score
        # Close the env and write monitor result info to disk
        print ("Your score: %d" % score)
        env.close()

    for file in range(10):
        with open(f'output{file}.txt', 'w') as filehandle:
            json.dump(dict[file], filehandle)
