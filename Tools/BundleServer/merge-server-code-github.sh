#!/bin/bash
#cp gitlab sparx_gam code to github
git="/usr/bin/git"
gitlab="/Users/jenkins/.jenkins/jobs/Development-Android-build/workspace"
github="/Users/jenkins/.jenkins/jobs/MergeServerCode-Gitlab-To-Github/workspace/sparx_gam"
cd $gitlab
$git checkout -f
$git branch --set-upstream-to=origin/feature/medusa_demo feature/medusa_demo
$git pull
rm -fr sparx_gam/node_modules sparx_gam/sparx
 
cp -a sparx_gam/ "$github"
$git checkout -f
cd $github

$git add --all
$git commit -m "Jira: GAM-0; Description: Update sparx_gam server code; Reviewer: none"
$git push
