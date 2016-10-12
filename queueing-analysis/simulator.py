from random import gammavariate, expovariate, uniform
from math import sqrt
import numpy
import matplotlib.pyplot as plt
from collections import deque

# -------- CONFIG -------- 
amountOfServers = 8

meanS, varS= amountOfServers*0.95, amountOfServers
meanA, varA= 1.0, 1.0

kS, tS = meanS**2/varS, varS/meanS
kA, tA = meanA**2/varA, varA/meanA

#kS = 16.0 # service: gamma distribution alpha
#tS = 1.0/sqrt(2.0) # service: gamma distribution beta
#kA = 1.0 # arrival: gamma distribution alpha
#tA = 1.0 # arrival: gamma distribution beta

obsL = 1.0 # random observation points: exp distribution lambda
observeArrivals = False
observeDepartures = False
verbose = False
maxSimulationTime = 10000
forceStop = True

# -------- FOR OTHER PLOTS THAN OF SIMULATION TIME (eg with service time mean or batch means) --------
mode = "batchMeans"
sys_simulationTime = 4000
sys_burnin = 1000
sys_meanS = [0.75, 0.8, 0.85, 0.9, 0.95, 1.0, 1.05, 1.1, 1.15, 1.2, 1.25]

# Total observations will be 20000
M = 20 # batch size
L = 1000 # batch amount
k_conf = 2.58 # for 99% confidence interval

# -------- INIT CODE (GLOBAL) -------- 
def init():
    global firstSimulation, arrivals, obsTimes, observations, batchDict
    firstSimulation = True
    arrivals = []
    obsTimes = []
    observations = {}
    observations["random"] = {}
    if observeArrivals == True:
        observations["arrival"] = {}
    if observeDepartures == True:
        observations["departure"] = {}
    if mode == "batchMeans":
        batchDict = {}
        
init()
schedulingDisciplines = ["FCFS", "LCFS", "SSTF", "ROS"]
obsVariables = [
    ["queueContent", "Queue Content"],
    ["systemContent", "System Content"],
    ["meanWaitingTime", "Mean Waiting Time"],
    ["meanSojournTime", "Mean Sojourn Time"],
    ["unfinishedWork", "Unfinished Work"],
    ["newWaitingTime", "Actual Waiting Time (served customers)"],
    ["newSojournTime", "Actual Sojourn Time (served customers)"],
    ["meanCompleteWaitingTime", "Mean Waiting Time (served customers)"],
    ["meanCompleteSojournTime", "Mean Sojourn Time (served customers)"]
]

# -------- INIT CODE (SIMULATION) -------- 
def reset():
    global agenda, continueDES, currentTime, queue, servers, done
    global doneSinceLastObsRandom, doneSinceLastObsArrival, doneSinceLastObsDeparture
    global observationsRandom, observationsArrival, observationsDeparture
    global currentL, currentM, batches, batchMeans
    agenda = {}
    continueDES = True
    currentTime = 0
    queue = deque([])
    servers = []
    done = []

    doneSinceLastObsRandom = []
    doneSinceLastObsArrival = []
    doneSinceLastObsDeparture = []

    observationsRandom = []
    observationsArrival = []
    observationsDeparture = []
    
    currentL = 0
    currentM = 0
    batches = []
    batchMeans = []

# -------- HELPER CLASSES -------- 
class Server:
    """
    Generic server object
    """
    def __init__(self, discipline):
        self.discipline = discipline
        self.content = None
        
    # Should only be called when the server is done serving a customer
    def depart(self):
        if(self.content is not None):
            if observeDepartures == True:
                observe("departure")
            done.append(self.content)
            doneSinceLastObsRandom.append(self.content)
            if observeArrivals == True:
                doneSinceLastObsArrival.append(self.content)
            if observeDepartures == True:
                doneSinceLastObsDeparture.append(self.content)
            self.content = None
    
    def serve(self, customer):
        self.content = customer
        customer.timeEnteredService = currentTime
        customer.waitingTime = currentTime - customer.arrivalTime
        customer.sojournTime = customer.waitingTime + customer.serviceTime
        schedule(currentTime + customer.serviceTime, departure, [self])

class Customer:
    """
    Generic customer object
    """
    def __init__(self, arrivalTime, serviceTime):
        self.arrivalTime = arrivalTime
        self.serviceTime = serviceTime
        
class Observation:
    """
    Generic observation object
    """
    def __init__(self, time):
        self.time = time
        # Observed variables are added in the observe(type) function


# -------- HELPER METHODS -------- 
def getFreeServer():
    for i in range(0, len(servers)):
        if servers[i].content is None:
            return servers[i]
    return None

def serviceTime():
    return gammavariate(kS, tS) # python's gammavariate pdf uses (k, theta) parameters instead of (alpha, beta)
    
def arrivalTime():
    return gammavariate(kA, tA)


# -------- SCHEDULING --------     
def schedule(time, event, arguments):
    if time in agenda:
        agenda[time].append([event, arguments])
    else:
        agenda[time] = [[event, arguments]]

def next():
    global currentTime, continueDES
    if len(agenda) > 0:
        currentTime = min(agenda)
        for event, args in agenda.pop(currentTime):
            event(*args)
    else:
        continueDES = False
    
def run ():
    while continueDES:
        next()


# -------- EVENTS -------- 
def arrival(cust):
    if firstSimulation == True:
        arrivals.append((currentTime, cust.serviceTime))
    queueSize = len(queue)
    if verbose == True:
        print("Arrival at " + str(currentTime))
    if observeArrivals == True:
        observe("arrival")
    if queueSize == 0 and getFreeServer() is not None:
        # No other customer in queue, this one is picked by default, whatever the scheduling discipline
        server = getFreeServer()
        server.serve(cust) # also schedules departure and sets customer waiting time, etc
    else:
        queue.append(cust)
    # Schedule arrivals only if this is the first simulation. Otherwise, we use the same arrivals as previous simulations.
    if firstSimulation == True:
        newCust = Customer(currentTime + arrivalTime(), serviceTime())
        if newCust.arrivalTime < maxSimulationTime or mode == "batchMeans":
            schedule(newCust.arrivalTime, arrival, [newCust])

def departure(server):
    queueSize = len(queue)
    if verbose == True:
        print("Departure at " + str(currentTime))
    server.depart()
    if queueSize > 0:
        discipline = server.discipline
        nextCust = None
        if discipline == "FCFS":
            nextCust = queue.popleft()
        elif discipline == "LCFS":
            nextCust = queue.pop()
        elif discipline == "SSTF":
            nextCust = queue[-1]
            for queueCust in queue:
                if queueCust.serviceTime < nextCust.serviceTime:
                    nextCust = queueCust
            queue.remove(nextCust)
        elif discipline == "ROS":
            # generate random
            randInd = int(round(uniform(0,queueSize-1)))
            nextCust = queue[randInd]
            del queue[randInd]
        server.serve(nextCust) # also schedules departure and sets customer waiting time, etc
        
def stop():
    global continueDES
    print("Stopped at " + str(currentTime))
    continueDES = False
    
def calcBatchMean(ind):
    batch = batches[ind]
    bobs = Observation(currentTime)
    bobs.time = sum(obs.time for obs in batch) / max(len(batch),1)
    bobs.queueContent = sum(obs.queueContent for obs in batch) / max(len(batch),1)
    bobs.systemContent = sum(obs.systemContent for obs in batch) / max(len(batch),1)
    bobs.meanCompleteWaitingTime = sum(obs.meanCompleteWaitingTime for obs in batch) / max(len(batch),1)
    bobs.meanWaitingTime = sum(obs.meanWaitingTime for obs in batch) / max(len(batch),1)
    bobs.meanCompleteSojournTime = sum(obs.meanCompleteSojournTime for obs in batch) / max(len(batch),1)
    bobs.meanSojournTime = sum(obs.meanSojournTime for obs in batch) / max(len(batch),1)
    bobs.unfinishedWork = sum(obs.unfinishedWork for obs in batch) / max(len(batch),1)
    bobs.newWaitingTime = sum(obs.newWaitingTime for obs in batch) / max(len(batch),1)
    bobs.newSojournTime = sum(obs.newSojournTime for obs in batch) / max(len(batch),1)
    batchMeans.append(bobs)
    
def observe(type):
    global currentL, currentM
    customersInService = [server.content for server in servers if server.content is not None]

    obs = Observation(currentTime)
    obs.queueContent = len(queue)
    obs.systemContent = obs.queueContent + len(customersInService)
    obs.meanCompleteWaitingTime = sum(cust.waitingTime for cust in done) / max(len(done),1) # max in denominator is there to prevent division by zero
    obs.meanWaitingTime = (sum(cust.waitingTime for cust in customersInService) + sum((currentTime - cust.arrivalTime) for cust in queue)) / max(len(customersInService) + len(queue),1)
    obs.meanCompleteSojournTime = sum(cust.sojournTime for cust in done) / max(len(done),1)
    obs.meanSojournTime = (sum(cust.sojournTime for cust in customersInService) + sum((currentTime - cust.arrivalTime + cust.serviceTime) for cust in queue)) / max(len(customersInService) + len(queue),1)
    obs.unfinishedWork = (sum(cust.serviceTime for cust in queue) + sum((cust.timeEnteredService + cust.serviceTime - currentTime) for cust in customersInService)) / amountOfServers
    if verbose == True:
        print("Observation at " + str(currentTime))
    
    if(type == "random"):
        if firstSimulation == True:
            obsTimes.append(currentTime)
            if len(agenda) > 0:
                if mode != "batchMeans" or currentL != L-1 or currentM != M-1:
                    schedule(currentTime + expovariate(obsL), observe, ["random"])
        # Additional measures
        if(len(doneSinceLastObsRandom) > 0):
            obs.newWaitingTime = sum(cust.waitingTime for cust in doneSinceLastObsRandom) / len(doneSinceLastObsRandom)
            obs.newSojournTime = sum(cust.sojournTime for cust in doneSinceLastObsRandom) / len(doneSinceLastObsRandom)
            del doneSinceLastObsRandom[:]
        else:
            if(len(observationsRandom) > 0):
                obs.newWaitingTime = observationsRandom[-1].newWaitingTime
                obs.newSojournTime = observationsRandom[-1].newSojournTime
            else:
                obs.newWaitingTime = 0
                obs.newSojournTime = 0
        observationsRandom.append(obs)
        if mode == "batchMeans":
            if currentM == 0:
                batches.append([])
            batches[currentL].append(obs)
            currentM = currentM + 1
            if currentM == M:
                calcBatchMean(currentL)
                currentM = 0
                currentL = currentL + 1
                if currentL == L:
                    schedule(currentTime + expovariate(obsL), stop, [])
    elif(type == "arrival"):
        if(len(doneSinceLastObsArrival) > 0):
            obs.newWaitingTime = sum(cust.waitingTime for cust in doneSinceLastObsArrival) / len(doneSinceLastObsArrival)
            obs.newSojournTime = sum(cust.sojournTime for cust in doneSinceLastObsArrival) / len(doneSinceLastObsArrival)
            del doneSinceLastObsArrival[:]
        else:
            if(len(observationsArrival) > 0):
                obs.newWaitingTime = observationsArrival[-1].newWaitingTime
                obs.newSojournTime = observationsArrival[-1].newSojournTime
            else:
                obs.newWaitingTime = 0
                obs.newSojournTime = 0
        observationsArrival.append(obs)
    elif(type == "departure"):
        if(len(doneSinceLastObsDeparture) > 0):
            obs.newWaitingTime = sum(cust.waitingTime for cust in doneSinceLastObsDeparture) / len(doneSinceLastObsDeparture)
            obs.newSojournTime = sum(cust.sojournTime for cust in doneSinceLastObsDeparture) / len(doneSinceLastObsDeparture)
            del doneSinceLastObsDeparture[:]
        else:
            if(len(observationsDeparture) > 0):
                obs.newWaitingTime = observationsDeparture[-1].newWaitingTime
                obs.newSojournTime = observationsDeparture[-1].newSojournTime
            else:
                obs.newWaitingTime = 0
                obs.newSojournTime = 0
        observationsDeparture.append(obs)

# -------- MAIN --------
def simulate():
    global firstSimulation
    for schedulingDiscipline in schedulingDisciplines:
        print("Beginning simulation for " + schedulingDiscipline)
        reset()
        for i in range(0,amountOfServers):
            server = Server(schedulingDiscipline)
            servers.append(server)
        if forceStop == True and mode != "batchMeans":
            schedule(maxSimulationTime, stop, [])
        if firstSimulation == True:
            firstCust = Customer(0, serviceTime())
            schedule(firstCust.arrivalTime, arrival, [firstCust])
            schedule(0, observe, ["random"])
        else:
            for arrivalTuple in arrivals:
                cust = Customer(arrivalTuple[0], arrivalTuple[1])
                schedule(cust.arrivalTime, arrival, [cust])
            for obsTime in obsTimes:
                schedule(obsTime, observe, ["random"])
        run()
        firstSimulation = False
        observations["random"][schedulingDiscipline] = observationsRandom;
        if observeArrivals == True:
            observations["arrival"][schedulingDiscipline] = observationsArrival;
        if observeDepartures == True:
            observations["departure"][schedulingDiscipline] = observationsDeparture;
        if mode == "batchMeans":
            batchDict[schedulingDiscipline] = batchMeans;

def plot():
    for obsType, obsDict in observations.items():
        for obsVariable in obsVariables:
            for schedulingDiscipline in schedulingDisciplines:
                obsList = obsDict[schedulingDiscipline]
                obsX = [obs.time for obs in obsList]
                obsY = [getattr(obs, obsVariable[0]) for obs in obsList]
                plt.plot(obsX, obsY, label=schedulingDiscipline)
            plt.xlabel("Simulation Time")
            plt.ylabel(obsVariable[1])
            #plt.axis([0, maxSimulationTime, 0, 200])
            plt.title(obsVariable[1] + " in G/G/" + str(amountOfServers) + " queues")
            plt.legend(loc="upper left" )
            plt.axis('auto')
            if forceStop == False:
                plt.axvline(x=maxSimulationTime, color='k', linestyle='dashed')
            paramStr = "serv" + str(amountOfServers)
            paramStr = paramStr + "_" + "meanA" + str("{0:.2f}".format(round(meanA,2)))
            paramStr = paramStr + "_" + "varA" + str("{0:.2f}".format(round(varA,2)))
            paramStr = paramStr + "_" + "meanS" + str("{0:.2f}".format(round(meanS,2)))
            paramStr = paramStr + "_" + "varS" + str("{0:.2f}".format(round(varS,2)))
            if forceStop == False:
                paramStr = paramStr + "_" + "noForceStop"
            paramStr = paramStr + "_" + obsType
            paramStr = paramStr + "_" + obsVariable[0]
            
            plt.savefig("plot_" + paramStr + ".pdf")
            plt.savefig("plot_" + paramStr + ".png", bbox_inches='tight')
            plt.close()

def plot_sys(obz):
    meanObservations = []
    for i, obzervations in enumerate(obz):
        meanObservations.append({})
        for obsType, obsDict in obzervations.items():
            meanObservations[i][obsType] = {}
            for obsVariable in obsVariables:
                meanObservations[i][obsType][obsVariable[0]] = {}
                for schedulingDiscipline in schedulingDisciplines:
                    obsList = obsDict[schedulingDiscipline]
                    obsY = [getattr(obs, obsVariable[0]) for obs in obsList if obs.time >= sys_burnin]
                    meanObservations[i][obsType][obsVariable[0]][schedulingDiscipline] = numpy.mean(obsY)
    for obsType, obsDict in obzervations.items():
        for obsVariable in obsVariables:
            for schedulingDiscipline in schedulingDisciplines:
                obsX = sys_meanS
                obsY = [meanObservations[i][obsType][obsVariable[0]][schedulingDiscipline] for i in range(0,len(sys_meanS))]
                plt.plot(obsX, obsY, label=schedulingDiscipline)
            plt.xlabel("Service time mean (times c)")
            plt.ylabel(obsVariable[1])
            plt.title(obsVariable[1] + " in G/G/" + str(amountOfServers) + " queues")
            plt.legend(loc="upper left" )
            plt.axis('auto')
            paramStr = "serv" + str(amountOfServers)
            paramStr = paramStr + "_" + "sysParams"
            if forceStop == False:
                paramStr = paramStr + "_" + "noForceStop"
            paramStr = paramStr + "_" + obsType
            paramStr = paramStr + "_" + obsVariable[0]
            
            plt.savefig("plot_" + paramStr + ".pdf")
            plt.savefig("plot_" + paramStr + ".png", bbox_inches='tight')
            plt.close()

def calculateBatchMeansVariance():
    for schedulingDiscipline in schedulingDisciplines:
        batchList = batchDict[schedulingDiscipline]
        for obsVariable in obsVariables:
            batchSum = sum(getattr(batch, obsVariable[0]) for batch in batchList)
            batchSum2 = sum(getattr(batch, obsVariable[0])**2 for batch in batchList)
            batchMean = batchSum/L
            batchVar = batchSum2/L - (batchSum/L)**2
            batchSD = sqrt(batchVar)
            
            print(schedulingDiscipline + " - " + obsVariable[1])
            print("BATCH MEAN: " + str("{0:.5f}".format(batchMean)) + " | VARIANCE: " + str("{0:.5f}".format(batchVar)) + \
            " | 99% CONFIDENCE INTERVAL: [" + str("{0:.5f}".format(batchMean-batchSD*k_conf)) + ", " + str("{0:.5f}".format(batchMean+batchSD*k_conf)) + "]")
            print()
            
if mode == "systemParams":
    maxSimulationTime = sys_simulationTime
    sys_obs = []
    for sys_mean in sys_meanS:
        init()
        meanS, varS= amountOfServers*sys_mean, amountOfServers
        kS, tS = meanS**2/varS, varS/meanS
        simulate()
        sys_obs.append(observations)
    plot_sys(sys_obs)
elif mode == "batchMeans":
    simulate()
    plot()
    calculateBatchMeansVariance()
else:
    simulate()
    plot()