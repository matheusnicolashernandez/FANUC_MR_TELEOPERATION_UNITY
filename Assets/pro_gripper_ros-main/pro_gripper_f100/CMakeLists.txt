cmake_minimum_required(VERSION 3.0.2)
project(pro_gripper_f100)
add_compile_options(-std=c++11)


## Find catkin and any catkin packagess
find_package(catkin REQUIRED COMPONENTS
  roscpp
  rospy
  std_msgs
  actionlib
  image_transport
  cv_bridge

  gripper_description
)

## Generate messages in the 'msg' folder
# add_message_files(
#   FILES
#   Message1.msg
#   Message2.msg
# )

## Generate services in the 'srv' folder
# add_service_files(
#   FILES
#   Service1.srv
#   Service2.srv
# )

## Generate actions in the 'action' folder
# add_action_files(
#   FILES
#   Action1.action
#   Action2.action
# )

## Generate added messages and services with any dependencies listed here
# generate_messages(
#   DEPENDENCIES
#   std_msgs
# )

## Generate dynamic reconfigure parameters in the 'cfg' folder
# generate_dynamic_reconfigure_options(
#   cfg/DynReconf1.cfg
#   cfg/DynReconf2.cfg
# )

## Declare a catkin package
catkin_package(
  CATKIN_DEPENDS std_msgs actionlib

#  INCLUDE_DIRS include
#  LIBRARIES pro_gripper_f100
#  CATKIN_DEPENDS roscpp rospy std_msgs
#  DEPENDS system_lib
)

## Specify additional locations of header files
## Your package locations should be listed before other locations
include_directories(include${catkin_INCLUDE_DIRS} ${OpenCV_INCLUDE_DIRS})

catkin_install_python(PROGRAMS
  scripts/force_gripper_slider.py
  DESTINATION ${CATKIN_PACKAGE_BIN_DESTINATION}

)

install(DIRECTORY launch DESTINATION ${CATKIN_PACKAGE_SHARE_DESTINATION}
  PATTERN "setup_assistant.launch" EXCLUDE)
install(DIRECTORY config DESTINATION ${CATKIN_PACKAGE_SHARE_DESTINATION})
