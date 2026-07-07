#!/usr/bin/env python3
# -*- coding:utf-8 -*-
"""[summary]
This file gets the joint angles of the force-controlled gripper in ROS,
and then uses the `elegripper` API to send it directly to the real gripper.
This file is the script related to [slider_control.launch].
Passable parameters:
port: serial protocol string. The default value is '/dev/ttyUSB0'
baud: serial protocol baud rate. The default value is 115200.
id: gripper ID, default is 14
"""
import os
import sys
sys.path.append(os.path.dirname(os.path.realpath(__file__)))
import rospy
from sensor_msgs.msg import JointState
from elegripper import Gripper

fg = None
gripper_value = []

def callback(data):
    global fg
    # rospy.loginfo(rospy.get_caller_id() + "%s", data.position)
    data_list = []
    for index, value in enumerate(data.position):
        data_list.append(round(value, 3))

    gripper_value = int(data_list[0]* 100)
    print("gripper_value:%s"%gripper_value)

    fg.set_gripper_value(gripper_value, speed=100)
    
def listener():
    global fg
    global gripper_value
   
    rospy.init_node("control_slider", anonymous=True)

    
    port = rospy.get_param("~port", "/dev/ttyACM0")
    baud = rospy.get_param("~baud", 115200)
    print(port, baud)
    fg = Gripper(port, baud, id=14)
    
    rospy.Subscriber("joint_states", JointState, callback)
 
    # spin() simply keeps python from exiting until this node is stopped
    print("spin ...")
    rospy.spin()


if __name__ == "__main__":
    listener()