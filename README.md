# Article reviewer and manager app.

This is an app written in **C#** for a grade during my Computer Science studies - it was developed by me and four other colleagues.
As far, as I know - it was never uploaded anywhere but on a e-learning platform of my former Uni, so I upload it here - it was also never on any other GithubRepo, thus there is no development history.

My role during the development of this app was pretty much everything in *ArticleDBLib* - all the models, methods reading/writing to the SQLite database and the migration creating the database on the first run.
Also, this is where I - after I did my part of the code - did my first software testing and I ensured that this app was working good enough so it can be graded and let us successfully pass the subject ;)

## What it does

As far, as I remember - this was an app that was locally governing the library of scientific articles - of course, it was working on ***SQLite*** database, and the it could hold following data:
- the title,
- the author or authors,
- the comments to those articles,
- their file location - so it could copy the file to its storage, as well,
- the keywords associated with those articles

This application also had an option to import entries from **LaTeX** and export its database to this format.

The UI was written in XAML.

There is also documentation in polish.

## Running an app

I haven't tried to launch it recently (or any time after getting a grade for it), but I believe it must be opened in Visual Studio and compiled there first.
