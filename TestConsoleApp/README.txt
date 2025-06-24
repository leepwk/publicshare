git config --global http.sslVerify false -- to allow cloning

-- Open developer tools in IE mode
%systemroot%\system32\f12\IEChooser.exe, and then click OK

-- GIT commands -- 
git help -a

-- cloning
git config --global http.sslVerify false
git clone <https://tfsappserver.bwd-rensburg/tfs/......>

-- local branch status
git status
git branch -a

-- verbose commits
git branch -a -vv

-- clean up local branches - remove dry-run for actual prune
git remote prune origin --dry-run
-- update local knowledge of remote branches
git remote update origin --prune 

-- Shows timeline commits across all branches
git show-branch

-- checkout new branch based off commit hash
git checkout -b <new-branch> <hash>

-- checkout new branch off a tag
git checkout tags/LIVE -b <new-branch>

-- List commits in 1.0_maint that haven't yet been merged to 1.0
git log <1.0_maint> ^^<1.0> --no-merges

-- List local commits not pushed
git log --branches --not --remotes
 
-- Find all branches containing the commits in remote 1.0_maint branch
git branch --all --contains <remotes/origin/1.0_maint>

-- branches with the commit id, -r means on remote repo
git branch -r --contains <commitId>

-- delete local branch 1.1
git branch -d <1.1>
-- delete remote branch for user created ones (no need to include remotes/origin in branch name)
git push origin --delete <1.1>

-- merge from <1.1> branch to current
git merge <1.1>

-- show current branch commits
git log --oneline

-- discard local commits ALL
git reset --hard @{u}

-- discard local commits most recent or number last commits
git reset --hard HEAD
git reset --hard HEAD~2

-- reset to a commit, including merge PR
git reset --hard <git-hash>

-- revert last commit that was pushed to remote, creates a new commit
git revert HEAD

-- GIT ADD A NEW LOCAL PROJECT TO REPO
-- git stage initial files
git init

-- in solution folder
git add .gitignore (or copy from other solutions)
git add .

-- first commit to branch
git commit -m "initial commit"

-- link to TFS git
git remote add origin "<url>"

-- to switch origin url
-- check current remote 
git remote -v
git remote set-url origin "<url>"

-- push a new branch to remote
git push --set-upstream origin master

-- push all
git push -u origin --all

-- amend last commit message
git commit --amend -m "New commit message"
git push --force origin <branch name>

-- stashing
git stash
git stash push -m "<meaningful description>"

git stash pop
git stash list
git stash drop <stashid>

-- only unstaged changes
git stash --keep-index 

-- reset when conflicts during pop
git reset --merge

-- tags
git tag LIVE <commitid> -f
git push origin -f --tags

-- fetch specific commit
git fetch origin <commit-hash>

-- cherry pick plus standard message
git cherry-pick -x <commithash>

-- rebase/squash commits
git rebase -i HEAD~n where n is the number of commits to squash
-- in the editor
pick 1234567 First commit
squash abcdefg Second commit
squash hijklmn Third commit
-- push to remote afterwards by force otherwise, the old commits will be synced back
git push --force

-- ANGULAR commands
> npm config set strict-ssl false - turn off ssl checks

> ng new <my-app>
-- installs
> npm install @angular/material 
> npm install @ng-bootstrap/ng-bootstrap 
> npm install ngx-toastr 
> npm install font-awesome 
> npm install ngx-loading 
> npm install xlsx - for importing csv etc. 

-- if NVM installed to control node version, prefix all commands with npx
> nvm list
> nvm install <version>
> nvm use <version>
> ng --version OR ng version

ngrx store for session and state management
> ng s[erve] api-server
> ng s[erve] <appname>
> ng s[erve] <appname> --configuration=dev = dev config

> ng g[enerate] c[omponent] <path/name>
> ng g[enerate] g[uard] <path/name>

> ng test
> ng test <appname> --watch=false (build tests)
> ng test <folderpath> --watch=false (build tests)
> ng build <appname> --aot (build from scratch)


-- AZURE database sql grant
CREATE USER [schemaprincipal] FROM EXTERNAL PROVIDER WITH DEFAULT_SCHEMA = [dbo];
GRANT CREATE TABLE TO [schemaprincipal] AS [dbo];
ALTER ROLE db_owner ADD MEMBER [schemaprincipal];