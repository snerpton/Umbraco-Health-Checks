# Umbraco-Health-Checks

Small project to create some basic generally useful HealthChecks for Umbraco projects. Project is not intended to develop any project/application specific HealthChecks.


##Umbraco Test Site - Details

- __Username__: admin@domain.com
- __Password__: password


##HealthChecks


###HumansTxtExists
Test if a `humans.txt` file exists in the website root directory. If the file doesn't exist a skeleton file will be created with the following content:

```
# humanstxt.org/
# The humans responsible & technology colophon

# TEAM

    <name> -- <role> -- <twitter>

# THANKS

    <name>

# TECHNOLOGY COLOPHON

    CSS3, HTML5
    Apache Server Configs, jQuery, Modernizr, Normalize.css
```

See http://humanstxt.org for more information.


###RobotsTxtExists 
Test if a `robots.txt` file exists in the website root directory. If the file doesn't exist a skeleton file will be created with the following content:

```
User-agent: *   # match all bots

# Disallow: /   # keep them out. Remove on go live!


# The following directories are all linked to in the frontend, but we probably don't want them indexed.
Disallow: / css
Disallow: / Css
Disallow: / less
Disallow: / Less
Disallow: / media
Disallow: / Media
Disallow: / scripts
Disallow: / Scripts
Disallow: / umbraco
Disallow: / Umbraco
```


