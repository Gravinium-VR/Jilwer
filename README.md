# Jilwer

Jilwer (jill-wear) is a toolkit that extends UdonSharp with additional editor
and runtime capabilities, reusable systems, and utility functions for VRChat
world and tooling development.

Documentation and Information: https://docs.gravinium.org/projects/jilwer

## How it works

Jilwer sits on top of UdonSharp instead of directly modifying it in any way.
It uses standard Unity and VRChat APIs to "add" functionality to UdonSharp
through often silly methods (don't look at the TypeRegistry T-T). More
information can be found in the docs!

The main component is the Jilwer runtime, which is what allows for the custom
interactions between UdonSharp and the Jilwer API... this means it creates
quite a few game objects during build time and runtime as a way to maintain
state and perform actions typically restricted by UdonSharp.

## AI Usage

I use LLMs here and there to help with context around code and generating parts
that I can then adapt into my own. I'm still against vibe coding and what not
so the most it'll ever be is chatbots but even then, there shouldn't be any direct
LLM output in the stable files. I also don't copy/paste code and instead only use
the output as reference for what I type myself. My bad code should be indication
enough to how much AI is used.

TLDR; I don't vibe code but do use output as reference for some parts of the code.

<details>
<summary>Old Statement</summary>
I'm generally against most AI generated code, especially code that ends up in
a stable version of Jilwer. They are useful tools though, and I myself sometimes
use them for small snippets of code. As such, I've adopted a system in which
I comment on any part of code that I've generated using AI. I expect the same
if anyone else decides to contribute to Jilwer but that will be documented more
in the contributing file.
</details>
