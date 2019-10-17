#!/bin/bash

set -e
run_cmd="dotnet watch -p Equinor.Procosys.Preservation.WebApi run"

dotnet restore

exec $run_cmd